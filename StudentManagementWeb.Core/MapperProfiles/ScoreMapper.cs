using StudentManagementWeb.Core.Entities;
using StudentManagementWeb.Core.Models.Scores;

namespace StudentManagementWeb.Core.MapperProfiles;

public static class ScoreMapper
{
    // Cần Include/gán Student và Subject trước khi gọi
    public static ScoreDto ToDto(this Score s) => new()
    {
        Id = s.Id,
        StudentId = s.StudentId,
        StudentCode = s.Student?.Code ?? string.Empty,
        StudentName = s.Student?.FullName ?? string.Empty,
        SubjectId = s.SubjectId,
        SubjectCode = s.Subject?.Code ?? string.Empty,
        SubjectName = s.Subject?.Name ?? string.Empty,
        Semester = s.Semester,
        SchoolYear = s.SchoolYear,
        Value = s.Value
    };

    public static Score ToEntity(this CreateScoreRequest r) => new()
    {
        StudentId = r.StudentId,
        SubjectId = r.SubjectId,
        Semester = r.Semester,
        SchoolYear = r.SchoolYear.Trim(),
        Value = r.Value
    };

    public static TranscriptItemDto ToTranscriptItem(this Score s) => new()
    {
        ScoreId = s.Id,
        SubjectId = s.SubjectId,
        SubjectCode = s.Subject?.Code ?? string.Empty,
        SubjectName = s.Subject?.Name ?? string.Empty,
        Semester = s.Semester,
        SchoolYear = s.SchoolYear,
        Value = s.Value
    };
}