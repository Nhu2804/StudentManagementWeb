using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;

namespace StudentManagementWeb.Core.SharedServices;

public class StudentCodeGenerator(AppDbContext db) : IStudentCodeGenerator
{
    public async Task<string> GenerateAsync(CancellationToken ct = default)
    {
        var prefix = $"{AppConstants.StudentCodePrefix}{DateTime.UtcNow.Year}";

        // IgnoreQueryFilters: tính cả học sinh đã xoá mềm để không trùng mã.
        var lastCode = await db.Students
            .IgnoreQueryFilters()
            .Where(s => s.Code.StartsWith(prefix))
            .OrderByDescending(s => s.Code)
            .Select(s => s.Code)
            .FirstOrDefaultAsync(ct);

        var next = 1;
        if (lastCode is not null && int.TryParse(lastCode[prefix.Length..], out var current))
            next = current + 1;

        return $"{prefix}{next:D4}";
    }
}