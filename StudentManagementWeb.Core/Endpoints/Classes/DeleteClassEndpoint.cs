using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Endpoints.Classes;

public class DeleteClassEndpoint(AppDbContext db) : Endpoint<IdRequest>
{
    public override void Configure()
    {
        Delete("/classes/{id}");
        
        Summary(s => s.Summary = "Xoá (mềm) lớp học – chỉ khi lớp không còn học sinh");
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var entity = await db.Classes.FirstOrDefaultAsync(c => c.Id == req.Id, ct);
        if (entity is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var hasStudents = await db.Students.AnyAsync(s => s.ClassId == req.Id, ct);
        if (hasStudents)
        {
            AddError(r => r.Id, ErrorMessages.ClassHasStudents);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        entity.IsDeleted = true;
        await db.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}