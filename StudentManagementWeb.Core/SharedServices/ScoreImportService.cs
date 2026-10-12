using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Scores;

namespace StudentManagementWeb.Core.SharedServices;

public class ScoreImportService(AppDbContext db, IScoreExcelService excel) : IScoreImportService
{
    private static readonly Regex SchoolYearRegex = new(@"^\d{4}-\d{4}$");

    public async Task<ScoreImportResult> ImportAsync(Stream stream, CancellationToken ct = default)
    {
        var scanned = excel.ReadRows(stream);
        if (scanned.Count == 0)
            throw new ExcelFileException(ErrorMessages.ImportNoData);

        var errors = new List<ImportRowError>();

        // 1) Kiểm tra định dạng từng dòng (ô trống, học kỳ, năm học, điểm).
        var parsed = new List<ImportScoreRow>();
        foreach (var row in scanned)
        {
            var message = TryParse(row, out var p);
            if (message is not null) errors.Add(Fail(row.RowNumber, row.Raw, message));
            else parsed.Add(p!);
        }

        // 2) Tra học sinh và môn học: mỗi loại một truy vấn cho cả file.
        var studentCodes = parsed.Select(p => p.StudentCode).Distinct().ToList();
        var subjectCodes = parsed.Select(p => p.SubjectCode).Distinct().ToList();

        var students = await db.Students
            .Where(s => studentCodes.Contains(s.Code))
            .Select(s => new { s.Id, s.Code })
            .ToDictionaryAsync(s => s.Code, s => s.Id, ct);

        var subjects = await db.Subjects
            .Where(s => subjectCodes.Contains(s.Code))
            .Select(s => new { s.Id, s.Code })
            .ToDictionaryAsync(s => s.Code, s => s.Id, ct);

        // 3) Không khớp thì báo lỗi; trùng trong file thì dòng đầu được ghi, các dòng sau báo lỗi.
        var toWrite = new List<(ImportScoreRow Row, Guid StudentId, Guid SubjectId)>();
        var seen = new Dictionary<string, int>();

        foreach (var p in parsed)
        {
            var problems = new List<string>();
            var hasStudent = students.TryGetValue(p.StudentCode, out var studentId);
            var hasSubject = subjects.TryGetValue(p.SubjectCode, out var subjectId);

            if (!hasStudent) problems.Add(string.Format(ErrorMessages.ImportStudentNotFound, p.StudentCode));
            if (!hasSubject) problems.Add(string.Format(ErrorMessages.ImportSubjectNotFound, p.SubjectCode));
            if (problems.Count > 0)
            {
                errors.Add(Fail(p.RowNumber, p.Raw, string.Join(" ", problems)));
                continue;
            }

            var key = $"{p.StudentCode}|{p.SubjectCode}|{p.Semester}|{p.SchoolYear}";
            if (seen.TryGetValue(key, out var firstRow))
            {
                errors.Add(Fail(p.RowNumber, p.Raw, string.Format(ErrorMessages.ImportDuplicateRow, firstRow)));
                continue;
            }

            seen[key] = p.RowNumber;
            toWrite.Add((p, studentId, subjectId));
        }

        // 4) Ghi: chưa có thì tạo, đã có thì cập nhật, đã xoá mềm thì khôi phục.
        var created = 0;
        var updated = 0;

        if (toWrite.Count > 0)
        {
            var studentIds = toWrite.Select(x => x.StudentId).Distinct().ToList();
            var subjectIds = toWrite.Select(x => x.SubjectId).Distinct().ToList();
            var years = toWrite.Select(x => x.Row.SchoolYear).Distinct().ToList();

            // IgnoreQueryFilters: unique index tính cả điểm đã xoá mềm.
            var existing = await db.Scores.IgnoreQueryFilters()
                .Where(s => studentIds.Contains(s.StudentId)
                         && subjectIds.Contains(s.SubjectId)
                         && years.Contains(s.SchoolYear))
                .ToListAsync(ct);

            var existingMap = existing.ToDictionary(
                s => (s.StudentId, s.SubjectId, s.Semester, s.SchoolYear));

            foreach (var (row, studentId, subjectId) in toWrite)
            {
                if (existingMap.TryGetValue((studentId, subjectId, row.Semester, row.SchoolYear), out var score))
                {
                    if (score.IsDeleted)
                    {
                        score.IsDeleted = false;   // khôi phục, tính là tạo mới
                        score.Value = row.Value;
                        created++;
                    }
                    else
                    {
                        score.Value = row.Value;
                        updated++;
                    }
                }
                else
                {
                    db.Scores.Add(row.ToEntity(studentId, subjectId));
                    created++;
                }
            }

            // Một lần lưu cho cả file: lỗi bất ngờ ở database thì không dòng nào bị ghi dở.
            await db.SaveChangesAsync(ct);
        }

        var ordered = errors.OrderBy(e => e.Row).ToList();

        return new ScoreImportResult
        {
            TotalRows = scanned.Count,
            Created = created,
            Updated = updated,
            Failed = ordered.Count,
            Errors = ordered
        };
    }

    /// <summary>Trả về thông báo lỗi, hoặc null nếu dòng hợp lệ (kèm dòng đã chuẩn hoá).</summary>
    private static string? TryParse(ScannedRow row, out ImportScoreRow? parsed)
    {
        parsed = null;
        var d = row.Raw;

        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(d.StudentCode)) missing.Add(ScoreImportConstants.Headers[0]);
        if (string.IsNullOrWhiteSpace(d.SubjectCode)) missing.Add(ScoreImportConstants.Headers[1]);
        if (string.IsNullOrWhiteSpace(d.Semester)) missing.Add(ScoreImportConstants.Headers[2]);
        if (string.IsNullOrWhiteSpace(d.SchoolYear)) missing.Add(ScoreImportConstants.Headers[3]);
        if (string.IsNullOrWhiteSpace(d.Score)) missing.Add(ScoreImportConstants.Headers[4]);
        if (missing.Count > 0)
            return string.Format(ErrorMessages.ImportRowMissing, string.Join(", ", missing));

        if (!int.TryParse(d.Semester, NumberStyles.None, CultureInfo.InvariantCulture, out var semester)
            || semester is < 1 or > 2)
            return ErrorMessages.ImportInvalidSemester;

        var schoolYear = d.SchoolYear.Trim();
        if (!SchoolYearRegex.IsMatch(schoolYear))
            return ErrorMessages.ImportInvalidSchoolYear;

        // Chấp nhận cả 8,5 và 8.5.
        var scoreText = d.Score.Trim().Replace(',', '.');
        if (!decimal.TryParse(scoreText,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out var value))
            return ErrorMessages.ImportScoreNotNumber;

        if (value < AppConstants.MinScore || value > AppConstants.MaxScore)
            return ErrorMessages.ImportScoreOutOfRange;

        parsed = new ImportScoreRow
        {
            RowNumber = row.RowNumber,
            Raw = d,
            StudentCode = d.StudentCode.Trim().ToUpperInvariant(),
            SubjectCode = d.SubjectCode.Trim().ToUpperInvariant(),
            Semester = semester,
            SchoolYear = schoolYear,
            Value = Math.Round(value, 2, MidpointRounding.AwayFromZero)   // cột Value chỉ lưu 2 chữ số thập phân
        };
        return null;
    }

    private static ImportRowError Fail(int row, ImportScoreRaw data, string message) => new()
    {
        Row = row,
        Data = data,
        Message = message
    };
}