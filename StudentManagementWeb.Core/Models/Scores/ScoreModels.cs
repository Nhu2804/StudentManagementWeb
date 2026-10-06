using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Models.Scores;

public class ScoreDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public Guid SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Semester { get; set; }
    public string SchoolYear { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class CreateScoreRequest
{
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public int Semester { get; set; }
    public string SchoolYear { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class UpdateScoreRequest
{
    public Guid Id { get; set; }   // lấy từ route /scores/{id}
    public decimal Value { get; set; }
}

public class ListScoresRequest : SieveRequest
{
    public Guid? StudentId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? ClassId { get; set; }
}

public class StudentTranscriptRequest
{
    public Guid StudentId { get; set; }   // lấy từ route /students/{studentId}/scores
    public int? Semester { get; set; }
    public string? SchoolYear { get; set; }
}

public class TranscriptItemDto
{
    public Guid ScoreId { get; set; }
    public Guid SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Semester { get; set; }
    public string SchoolYear { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class StudentTranscriptDto
{
    public Guid StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public IReadOnlyList<TranscriptItemDto> Items { get; set; } = Array.Empty<TranscriptItemDto>();
    public decimal? AverageScore { get; set; }
    public string? Classification { get; set; }
}