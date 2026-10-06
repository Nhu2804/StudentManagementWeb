namespace StudentManagementWeb.Core.SharedServices;

public interface IStudentCodeGenerator
{
    /// <summary>Sinh mã học sinh kế tiếp theo năm hiện tại, ví dụ HS20260001.</summary>
    Task<string> GenerateAsync(CancellationToken ct = default);
}