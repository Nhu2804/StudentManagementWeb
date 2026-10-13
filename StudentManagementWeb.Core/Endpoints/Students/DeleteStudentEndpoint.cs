using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Endpoints.Students;

[HttpDelete(ApiRoutes.Students.Delete)]
public class DeleteStudentEndpoint(AppDbContext db) : Endpoint<IdRequest>
{
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

public class DeleteStudentSummary : Summary<DeleteStudentEndpoint>
{
    public DeleteStudentSummary()
    {
        Summary = "Xoá học sinh";
    }
}