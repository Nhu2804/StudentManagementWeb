using StudentManagementWeb.Core.Entities;
using StudentManagementWeb.Core.Models.Subjects;

namespace StudentManagementWeb.Core.MapperProfiles;

public static class SubjectMapper
{
    public static SubjectDto ToDto(this Subject s) => new()
    {
        Id = s.Id,
        Code = s.Code,
        Name = s.Name
    };

    public static Subject ToEntity(this CreateSubjectRequest r) => new()
    {
        Code = r.Code.Trim().ToUpperInvariant(),
        Name = r.Name.Trim()
    };

    public static void ApplyTo(this UpdateSubjectRequest r, Subject s)
    {
        s.Code = r.Code.Trim().ToUpperInvariant();
        s.Name = r.Name.Trim();
    }
}