using StudentManagement.Core.Entities;
using StudentManagementWeb.Core.Constants;

namespace StudentManagementWeb.Core.Entities;

public class Student : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;
}