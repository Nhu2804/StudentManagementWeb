using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Classes;

namespace StudentManagementWeb.Core.Endpoints.Classes;

public class CreateClassValidator : Validator<CreateClassRequest>
{
    public CreateClassValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Grade).InclusiveBetween(1, 12);
        RuleFor(x => x.SchoolYear).NotEmpty().Matches(@"^\d{4}-\d{4}$")
            .WithMessage("Năm học phải có dạng 2026-2027.");
    }
}

public class CreateClassEndpoint(AppDbContext db) : Endpoint<CreateClassRequest, ClassDto>
{
    public override void Configure()
    {
        Post("/classes");
        
        Summary(s => s.Summary = "Tạo lớp học mới");
    }

    public override async Task HandleAsync(CreateClassRequest req, CancellationToken ct)
    {
        var name = req.Name.Trim();
        var schoolYear = req.SchoolYear.Trim();

        var exists = await db.Classes.IgnoreQueryFilters().AnyAsync(c => c.Name == name && c.SchoolYear == schoolYear, ct);
        if (exists)
        {
            AddError(r => r.Name, ErrorMessages.ClassNameExists);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var entity = req.ToEntity();
        db.Classes.Add(entity);
        await db.SaveChangesAsync(ct);

        await Send.ResponseAsync(entity.ToDto(), 201, ct);
    }
}