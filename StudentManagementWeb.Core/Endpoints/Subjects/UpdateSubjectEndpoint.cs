using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Subjects;

namespace StudentManagementWeb.Core.Endpoints.Subjects;

public class UpdateSubjectValidator : Validator<UpdateSubjectRequest>
{
    public UpdateSubjectValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class UpdateSubjectEndpoint(AppDbContext db) : Endpoint<UpdateSubjectRequest, SubjectDto>
{
    public override void Configure()
    {
        Put("/subjects/{id}");
        Roles(AppRoles.Admin);

        Summary(s => s.Summary = "Cập nhật môn học");
    }

    public override async Task HandleAsync(UpdateSubjectRequest req, CancellationToken ct)
    {
        var subject = await db.Subjects.FirstOrDefaultAsync(s => s.Id == req.Id, ct);
        if (subject is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var code = req.Code.Trim().ToUpperInvariant();
        var codeTaken = await db.Subjects.IgnoreQueryFilters()
            .AnyAsync(s => s.Code == code && s.Id != req.Id, ct);
        if (codeTaken)
        {
            AddError(r => r.Code, ErrorMessages.SubjectCodeExists);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        req.ApplyTo(subject);
        await db.SaveChangesAsync(ct);

        await Send.OkAsync(subject.ToDto(), ct);
    }
}