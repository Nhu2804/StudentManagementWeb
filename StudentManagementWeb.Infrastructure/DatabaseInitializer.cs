using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.Entities;

namespace StudentManagementWeb.Infrastructure;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (db.Database.GetMigrations().Any())
            await db.Database.MigrateAsync();
        else
            await db.Database.EnsureCreatedAsync();

        await SeedAsync(db);
    }

    private static async Task SeedAsync(AppDbContext db)
    {
        // IgnoreQueryFilters: bản ghi xoá mềm vẫn tính là "đã có dữ liệu" nên không seed lại.
        if (!await db.Subjects.IgnoreQueryFilters().AnyAsync())
        {
            db.Subjects.AddRange(
                new Subject { Code = "TOAN", Name = "Toán" },
                new Subject { Code = "VAN", Name = "Ngữ văn" },
                new Subject { Code = "ANH", Name = "Tiếng Anh" },
                new Subject { Code = "LY", Name = "Vật lý" },
                new Subject { Code = "HOA", Name = "Hóa học" },
                new Subject { Code = "SINH", Name = "Sinh học" },
                new Subject { Code = "SU", Name = "Lịch sử" },
                new Subject { Code = "DIA", Name = "Địa lý" });
        }

        if (!await db.Classes.IgnoreQueryFilters().AnyAsync())
        {
            db.Classes.AddRange(
                new Class { Name = "10A1", Grade = 10, SchoolYear = "2026-2027" },
                new Class { Name = "10A2", Grade = 10, SchoolYear = "2026-2027" },
                new Class { Name = "11A1", Grade = 11, SchoolYear = "2026-2027" });
        }

        var admin = await db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Username == "admin");
        if (admin is null)
        {
            db.Users.Add(new AppUser
            {
                Username = "admin",
                FullName = "Quản trị viên",
                Role = AppRoles.Admin,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123")
            });
        }
        else if (string.IsNullOrEmpty(admin.Role))
        {
            admin.Role = AppRoles.Admin;
        }

        if (!await db.Users.IgnoreQueryFilters().AnyAsync(u => u.Username == "teacher"))
        {
            db.Users.Add(new AppUser
            {
                Username = "teacher",
                FullName = "Giáo viên mẫu",
                Role = AppRoles.Teacher,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123")
            });
        }

        await db.SaveChangesAsync();
    }
}