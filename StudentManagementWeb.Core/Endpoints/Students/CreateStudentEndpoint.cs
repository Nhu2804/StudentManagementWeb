using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Students;
using StudentManagementWeb.Core.SharedServices;

namespace StudentManagementWeb.Core.Endpoints.Students;

public class CreateStudentValidator : Validator<CreateStudentRequest>
{
    public CreateStudentValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DateOfBirth).LessThan(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(20);
        RuleFor(x => x.ClassId).NotEmpty();
    }
}

public class CreateStudentEndpoint(AppDbContext db, IStudentCodeGenerator codeGenerator)
    : Endpoint<CreateStudentRequest, StudentDto>
{
    public override void Configure()
    {
        Post("/students");
        
        Summary(s => s.Summary = "Thêm học sinh mới");
    }

    public override async Task HandleAsync(CreateStudentRequest req, CancellationToken ct)
    {
        var classExists = await db.Classes.AnyAsync(c => c.Id == req.ClassId, ct);
        if (!classExists)
        {
            AddError(r => r.ClassId, ErrorMessages.ClassNotFound);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var code = await codeGenerator.GenerateAsync(ct);
        var student = req.ToEntity(code);

        db.Students.Add(student);
        await db.SaveChangesAsync(ct);

        await db.Entry(student).Reference(s => s.Class).LoadAsync(ct);
        await Send.ResponseAsync(student.ToDto(), 201, ct);
    }
}