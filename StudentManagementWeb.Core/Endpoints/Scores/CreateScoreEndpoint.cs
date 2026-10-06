using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.Entities;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Scores;

namespace StudentManagementWeb.Core.Endpoints.Scores;

public class CreateScoreValidator : Validator<CreateScoreRequest>
{
    public CreateScoreValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.Semester).InclusiveBetween(1, 2)
            .WithMessage("Học kỳ phải là 1 hoặc 2.");
        RuleFor(x => x.SchoolYear).NotEmpty().Matches(@"^\d{4}-\d{4}$")
            .WithMessage("Năm học phải có dạng 2026-2027.");
        RuleFor(x => x.Value).InclusiveBetween(AppConstants.MinScore, AppConstants.MaxScore)
            .WithMessage($"Điểm phải nằm trong khoảng {AppConstants.MinScore} - {AppConstants.MaxScore}.");
    }
}

public class CreateScoreEndpoint(AppDbContext db) : Endpoint<CreateScoreRequest, ScoreDto>
{
    public override void Configure()
    {
        Post("/scores");
        
        Summary(s => s.Summary = "Nhập điểm cho học sinh (mỗi môn/học kỳ/năm học một điểm)");
    }

    public override async Task HandleAsync(CreateScoreRequest req, CancellationToken ct)
    {
        var student = await db.Students.FirstOrDefaultAsync(s => s.Id == req.StudentId, ct);
        if (student is null)
        {
            AddError(r => r.StudentId, ErrorMessages.StudentNotFound);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var subject = await db.Subjects.FirstOrDefaultAsync(s => s.Id == req.SubjectId, ct);
        if (subject is null)
        {
            AddError(r => r.SubjectId, ErrorMessages.SubjectNotFound);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var schoolYear = req.SchoolYear.Trim();

        // IgnoreQueryFilters: unique index tính cả điểm đã xoá mềm
        var existing = await db.Scores.IgnoreQueryFilters().FirstOrDefaultAsync(s =>
            s.StudentId == req.StudentId &&
            s.SubjectId == req.SubjectId &&
            s.Semester == req.Semester &&
            s.SchoolYear == schoolYear, ct);

        if (existing is { IsDeleted: false })
        {
            AddError(r => r.SubjectId, ErrorMessages.ScoreExists);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        Score score;
        if (existing is not null)
        {
            // Điểm cũ đã xoá mềm -> khôi phục với giá trị mới
            existing.IsDeleted = false;
            existing.Value = req.Value;
            score = existing;
        }
        else
        {
            score = req.ToEntity();
            db.Scores.Add(score);
        }

        await db.SaveChangesAsync(ct);

        score.Student = student;
        score.Subject = subject;
        await Send.ResponseAsync(score.ToDto(), 201, ct);
    }
}