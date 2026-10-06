using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Endpoints.Subjects;

public class DeleteSubjectEndpoint(AppDbContext db) : Endpoint<IdRequest>
{
    public override void Configure()
    {
        Delete("/subjects/{id}");
        Roles(AppRoles.Admin);

        Summary(s => s.Summary = "Xoá môn học – chỉ khi chưa có điểm");
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var subject = await db.Subjects.FirstOrDefaultAsync(s => s.Id == req.Id, ct);
        if (subject is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var hasScores = await db.Scores.AnyAsync(s => s.SubjectId == req.Id, ct);
        if (hasScores)
        {
            AddError(r => r.Id, ErrorMessages.SubjectHasScores);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        subject.IsDeleted = true;
        await db.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}