using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Classes;

namespace StudentManagementWeb.Core.Endpoints.Classes;

public class UpdateClassValidator : Validator<UpdateClassRequest>
{
    public UpdateClassValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Grade).InclusiveBetween(1, 12);
        RuleFor(x => x.SchoolYear).NotEmpty().Matches(@"^\d{4}-\d{4}$")
            .WithMessage("Năm học phải có dạng 2026-2027.");
    }
}

public class UpdateClassEndpoint(AppDbContext db) : Endpoint<UpdateClassRequest, ClassDto>
{
    public override void Configure()
    {
        Put("/classes/{id}");
        
        Summary(s => s.Summary = "Cập nhật lớp học");
    }

    public override async Task HandleAsync(UpdateClassRequest req, CancellationToken ct)
    {
        var entity = await db.Classes.FirstOrDefaultAsync(c => c.Id == req.Id, ct);
        if (entity is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var name = req.Name.Trim();
        var schoolYear = req.SchoolYear.Trim();

        // Loại trừ chính lớp đang sửa; IgnoreQueryFilters vì unique index tính cả lớp đã xoá mềm.
        var taken = await db.Classes.IgnoreQueryFilters()
            .AnyAsync(c => c.Name == name && c.SchoolYear == schoolYear && c.Id != req.Id, ct);
        if (taken)
        {
            AddError(r => r.Name, ErrorMessages.ClassNameExists);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        req.ApplyTo(entity);
        await db.SaveChangesAsync(ct);

        var studentCount = await db.Students.CountAsync(s => s.ClassId == entity.Id, ct);
        await Send.OkAsync(entity.ToDto(studentCount), ct);
    }
}