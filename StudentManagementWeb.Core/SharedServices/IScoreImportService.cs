using StudentManagementWeb.Core.Models.Scores;

namespace StudentManagementWeb.Core.SharedServices;

public interface IScoreImportService
{
    /// <summary>Import điểm từ file Excel. Ném ExcelFileException nếu cả file không dùng được.</summary>
    Task<ScoreImportResult> ImportAsync(Stream stream, CancellationToken ct = default);
}