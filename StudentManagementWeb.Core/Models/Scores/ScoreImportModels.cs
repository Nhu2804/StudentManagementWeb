using Microsoft.AspNetCore.Http;

namespace StudentManagementWeb.Core.Models.Scores;

public class ScoreImportRequest
{
    public IFormFile File { get; set; } = null!;
}

/// <summary>Object bắt được lúc quét file: giá trị thô đúng như trong ô Excel.</summary>
public class ImportScoreRaw
{
    public string StudentCode { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public string SchoolYear { get; set; } = string.Empty;
    public string Score { get; set; } = string.Empty;
}

/// <summary>Dòng vừa quét từ Excel, kèm số dòng.</summary>
public record ScannedRow(int RowNumber, ImportScoreRaw Raw);

/// <summary>Dòng đã qua kiểm tra định dạng, mã đã chuẩn hoá (cắt khoảng trắng, chữ hoa).</summary>
public class ImportScoreRow
{
    public int RowNumber { get; set; }
    public ImportScoreRaw Raw { get; set; } = new();
    public string StudentCode { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public int Semester { get; set; }
    public string SchoolYear { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class ImportRowError
{
    public int Row { get; set; }
    public ImportScoreRaw Data { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class ScoreImportResult
{
    public int TotalRows { get; set; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public int Failed { get; set; }
    public IReadOnlyList<ImportRowError> Errors { get; set; } = Array.Empty<ImportRowError>();
}