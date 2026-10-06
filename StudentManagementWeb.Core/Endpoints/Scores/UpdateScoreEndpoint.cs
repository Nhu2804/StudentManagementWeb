using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Scores;

namespace StudentManagementWeb.Core.Endpoints.Scores;

public class UpdateScoreValidator : Validator<UpdateScoreRequest>
{
    public UpdateScoreValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Value).InclusiveBetween(AppConstants.MinScore, AppConstants.MaxScore)
            .WithMessage($"Điểm phải nằm trong khoảng {AppConstants.MinScore} - {AppConstants.MaxScore}.");
    }
}

public class UpdateScoreEndpoint(AppDbContext db) : Endpoint<UpdateScoreRequest, ScoreDto>
{
    public override void Configure()
    {
        Put("/scores/{id}");
        Roles(AppRoles.Admin, AppRoles.Teacher);

        Summary(s => s.Summary = "Sửa giá trị điểm");
    }

    public override async Task HandleAsync(UpdateScoreRequest req, CancellationToken ct)
    {
        var score = await db.Scores
            .Include(s => s.Student)
            .Include(s => s.Subject)
            .FirstOrDefaultAsync(s => s.Id == req.Id, ct);

        if (score is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        score.Value = req.Value;
        await db.SaveChangesAsync(ct);

        await Send.OkAsync(score.ToDto(), ct);
    }
}