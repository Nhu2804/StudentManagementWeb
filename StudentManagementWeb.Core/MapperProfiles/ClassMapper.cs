using StudentManagementWeb.Core.Entities;
using StudentManagementWeb.Core.Models.Classes;

namespace StudentManagementWeb.Core.MapperProfiles;

public static class ClassMapper
{
    public static ClassDto ToDto(this Class c, int studentCount = 0) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Grade = c.Grade,
        SchoolYear = c.SchoolYear,
        StudentCount = studentCount
    };

    public static Class ToEntity(this CreateClassRequest r) => new()
    {
        Name = r.Name.Trim(),
        Grade = r.Grade,
        SchoolYear = r.SchoolYear.Trim()
    };

    public static void ApplyTo(this UpdateClassRequest r, Class c)
    {
        c.Name = r.Name.Trim();
        c.Grade = r.Grade;
        c.SchoolYear = r.SchoolYear.Trim();
    }
}