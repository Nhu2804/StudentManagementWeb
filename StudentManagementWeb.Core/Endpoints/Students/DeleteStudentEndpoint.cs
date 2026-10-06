using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Endpoints.Students;

public class DeleteStudentEndpoint(AppDbContext db) : Endpoint<IdRequest>
{
    public override void Configure()
    {
        Delete("/students/{id}");
        
        Summary(s => s.Summary = "Xoá (mềm) học sinh");
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var student = await db.Students.FirstOrDefaultAsync(s => s.Id == req.Id, ct);
        if (student is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        student.IsDeleted = true;
        await db.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}