using StudentManagement.Core.Entities;

namespace StudentManagementWeb.Core.Entities;

public class AppUser : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    /// <summary>Chỉ dùng cho tài khoản vai trò Student: trỏ tới học sinh tương ứng.</summary>
    public Guid? StudentId { get; set; }
    public Student? Student { get; set; }
}