using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Students;

namespace StudentManagementWeb.Core.Endpoints.Students;

public class UpdateStudentValidator : Validator<UpdateStudentRequest>
{
    public UpdateStudentValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DateOfBirth).LessThan(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(20);
        RuleFor(x => x.ClassId).NotEmpty();
    }
}

public class UpdateStudentEndpoint(AppDbContext db) : Endpoint<UpdateStudentRequest, StudentDto>
{
    public override void Configure()
    {
        Put("/students/{id}");
        
        Summary(s => s.Summary = "Cập nhật thông tin học sinh");
    }

    public override async Task HandleAsync(UpdateStudentRequest req, CancellationToken ct)
    {
        var student = await db.Students.FirstOrDefaultAsync(s => s.Id == req.Id, ct);
        if (student is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var classExists = await db.Classes.AnyAsync(c => c.Id == req.ClassId, ct);
        if (!classExists)
        {
            AddError(r => r.ClassId, ErrorMessages.ClassNotFound);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        req.ApplyTo(student);
        await db.SaveChangesAsync(ct);

        await db.Entry(student).Reference(s => s.Class).LoadAsync(ct);
        await Send.OkAsync(student.ToDto(), ct);
    }
}