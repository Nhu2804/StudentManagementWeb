using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Scores;
using StudentManagementWeb.Core.SharedServices;

namespace StudentManagementWeb.Core.Endpoints.Scores;

// Hai đường dẫn cùng một endpoint:
//  - Transcript (/students/{studentId}/scores): Admin/Teacher xem bất kỳ học sinh nào
//  - Me (/scores/me)                          : học sinh xem điểm của chính mình
[HttpGet(ApiRoutes.Scores.Transcript, ApiRoutes.Scores.Me)]
public class GetStudentTranscriptEndpoint(AppDbContext db)
    : Endpoint<StudentTranscriptRequest, StudentTranscriptDto>
{
    public override async Task HandleAsync(StudentTranscriptRequest req, CancellationToken ct)
    {
        var studentId = req.StudentId ?? Guid.Empty;

        if (User.IsInRole(AppRoles.Student))
        {
            // Học sinh: luôn lấy id từ token, bỏ qua id gửi lên nên không xem được người khác.
            if (!Guid.TryParse(User.FindFirst("StudentId")?.Value, out studentId))
            {
                await Send.ForbiddenAsync(ct);
                return;
            }
        }
        else if (studentId == Guid.Empty)
        {
            AddError(r => r.StudentId, "Cần truyền studentId.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var student = await db.Students
            .AsNoTracking()
            .Include(s => s.Class)
            .FirstOrDefaultAsync(s => s.Id == studentId, ct);

        if (student is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var query = db.Scores.AsNoTracking()
            .Include(s => s.Subject)
            .Where(s => s.StudentId == studentId);

        if (req.Semester is not null)
            query = query.Where(s => s.Semester == req.Semester);

        if (!string.IsNullOrWhiteSpace(req.SchoolYear))
        {
            var year = req.SchoolYear.Trim();
            query = query.Where(s => s.SchoolYear == year);
        }

        var scores = await query
            .OrderByDescending(s => s.SchoolYear)
            .ThenBy(s => s.Semester)
            .ThenBy(s => s.Subject.Name)
            .ToListAsync(ct);

        decimal? average = scores.Count == 0 ? null : Math.Round(scores.Average(s => s.Value), 2);

        await Send.OkAsync(new StudentTranscriptDto
        {
            StudentId = student.Id,
            StudentCode = student.Code,
            StudentName = student.FullName,
            ClassName = student.Class?.Name ?? string.Empty,
            Items = scores.Select(s => s.ToTranscriptItem()).ToList(),
            AverageScore = average,
            Classification = average is null ? null : GradeClassifier.Classify(average.Value)
        }, ct);
    }
}

public class GetStudentTranscriptSummary : Summary<GetStudentTranscriptEndpoint>
{
    public GetStudentTranscriptSummary()
    {
        Summary = "Bảng điểm học sinh, kèm điểm trung bình và xếp loại";
    }
}