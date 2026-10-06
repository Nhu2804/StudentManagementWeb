using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Common;
using StudentManagementWeb.Core.Models.Students;

namespace StudentManagementWeb.Core.Endpoints.Students;

public class GetStudentByIdEndpoint(AppDbContext db) : Endpoint<IdRequest, StudentDto>
{
    public override void Configure()
    {
        Get("/students/{id}");
        Roles(AppRoles.Admin, AppRoles.Teacher);

        Summary(s => s.Summary = "Lấy thông tin một học sinh");
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var student = await db.Students
            .AsNoTracking()
            .Include(s => s.Class)
            .FirstOrDefaultAsync(s => s.Id == req.Id, ct);

        if (student is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(student.ToDto(), ct);
    }
}