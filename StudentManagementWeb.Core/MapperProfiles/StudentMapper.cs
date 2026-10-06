using StudentManagementWeb.Core.Entities;
using StudentManagementWeb.Core.Models.Students;

namespace StudentManagementWeb.Core.MapperProfiles;

public static class StudentMapper
{
    public static StudentDto ToDto(this Student s) => new()
    {
        Id = s.Id,
        Code = s.Code,
        FullName = s.FullName,
        DateOfBirth = s.DateOfBirth,
        Gender = s.Gender,
        Email = s.Email,
        Phone = s.Phone,
        Address = s.Address,
        ClassId = s.ClassId,
        ClassName = s.Class?.Name ?? string.Empty
    };

    public static Student ToEntity(this CreateStudentRequest r, string code) => new()
    {
        Code = code,
        FullName = r.FullName.Trim(),
        DateOfBirth = r.DateOfBirth,
        Gender = r.Gender,
        Email = r.Email?.Trim(),
        Phone = r.Phone?.Trim(),
        Address = r.Address?.Trim(),
        ClassId = r.ClassId
    };

    public static void ApplyTo(this UpdateStudentRequest r, Student s)
    {
        s.FullName = r.FullName.Trim();
        s.DateOfBirth = r.DateOfBirth;
        s.Gender = r.Gender;
        s.Email = r.Email?.Trim();
        s.Phone = r.Phone?.Trim();
        s.Address = r.Address?.Trim();
        s.ClassId = r.ClassId;
    }
}