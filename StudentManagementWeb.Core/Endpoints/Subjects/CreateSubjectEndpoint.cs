using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Subjects;

namespace StudentManagementWeb.Core.Endpoints.Subjects;

public class CreateSubjectValidator : Validator<CreateSubjectRequest>
{
    public CreateSubjectValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class CreateSubjectEndpoint(AppDbContext db) : Endpoint<CreateSubjectRequest, SubjectDto>
{
    public override void Configure()
    {
        Post("/subjects");
        Roles(AppRoles.Admin);

        Summary(s => s.Summary = "Thêm môn học mới");
    }

    public override async Task HandleAsync(CreateSubjectRequest req, CancellationToken ct)
    {
        var code = req.Code.Trim().ToUpperInvariant();

        // IgnoreQueryFilters: unique index trong DB tính cả bản ghi đã xoá mềm
        var exists = await db.Subjects.IgnoreQueryFilters().AnyAsync(s => s.Code == code, ct);
        if (exists)
        {
            AddError(r => r.Code, ErrorMessages.SubjectCodeExists);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var entity = req.ToEntity();
        db.Subjects.Add(entity);
        await db.SaveChangesAsync(ct);

        await Send.ResponseAsync(entity.ToDto(), 201, ct);
    }
}