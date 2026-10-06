using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Endpoints.Scores;

public class DeleteScoreEndpoint(AppDbContext db) : Endpoint<IdRequest>
{
    public override void Configure()
    {
        Delete("/scores/{id}");
        
        Summary(s => s.Summary = "Xoá (mềm) điểm");
    }

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        var score = await db.Scores.FirstOrDefaultAsync(s => s.Id == req.Id, ct);
        if (score is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        score.IsDeleted = true;
        await db.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}