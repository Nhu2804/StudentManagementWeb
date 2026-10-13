using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Common;
using StudentManagementWeb.Core.Models.Subjects;

namespace StudentManagementWeb.Core.Endpoints.Subjects;

[HttpGet(ApiRoutes.Subjects.GetById)]
public class GetSubjectByIdEndpoint(AppDbContext db) : Endpoint<IdRequest, SubjectDto>
{
    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var subject = await db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == req.Id, ct);
        if (subject is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(subject.ToDto(), ct);
    }
}

public class GetSubjectByIdSummary : Summary<GetSubjectByIdEndpoint>
{
    public GetSubjectByIdSummary()
    {
        Summary = "Lấy thông tin một môn học";
    }
}