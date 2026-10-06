using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Models.Students;

public class StudentDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
}

public class CreateStudentRequest
{
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Guid ClassId { get; set; }
}

public class UpdateStudentRequest : CreateStudentRequest
{
    /// <summary>Lấy từ route /students/{id}.</summary>
    public Guid Id { get; set; }
}

public class ListStudentsRequest : SieveRequest;