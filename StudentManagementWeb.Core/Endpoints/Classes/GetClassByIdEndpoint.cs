using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Classes;
using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Endpoints.Classes;

[HttpGet(ApiRoutes.Classes.GetById)]
public class GetClassByIdEndpoint(AppDbContext db) : Endpoint<IdRequest, ClassDto>
{
    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var row = await db.Classes
            .AsNoTracking()
            .Where(c => c.Id == req.Id)
            .Select(c => new { Class = c, StudentCount = c.Students.Count })
            .FirstOrDefaultAsync(ct);

        if (row is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(row.Class.ToDto(row.StudentCount), ct);
    }
}

public class GetClassByIdSummary : Summary<GetClassByIdEndpoint>
{
    public GetClassByIdSummary()
    {
        Summary = "Lấy thông tin một lớp học (kèm sĩ số)";
    }
}