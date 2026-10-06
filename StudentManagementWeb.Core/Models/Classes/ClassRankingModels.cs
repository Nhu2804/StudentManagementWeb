namespace StudentManagementWeb.Core.Models.Classes;

public class ClassRankingRequest
{
    /// <summary>Lấy từ route /classes/{classId}/ranking.</summary>
    public Guid ClassId { get; set; }
    public int? Semester { get; set; }
    public string? SchoolYear { get; set; }
}

public class RankingItemDto
{
    /// <summary>Null nếu học sinh chưa có điểm nào trong phạm vi lọc.</summary>
    public int? Rank { get; set; }
    public Guid StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int ScoreCount { get; set; }
    public decimal? AverageScore { get; set; }
    public string? Classification { get; set; }
}

public class ClassRankingDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public IReadOnlyList<RankingItemDto> Items { get; set; } = Array.Empty<RankingItemDto>();
}