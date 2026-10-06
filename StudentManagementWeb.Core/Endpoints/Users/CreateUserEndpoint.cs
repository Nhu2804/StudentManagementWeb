using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.Entities;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Auth;

namespace StudentManagementWeb.Core.Endpoints.Users;

public class CreateUserValidator : Validator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().Length(3, 50)
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("Tên đăng nhập chỉ gồm chữ, số, dấu chấm, gạch dưới, gạch ngang.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Role).Must(r => AppRoles.All.Contains(r))
            .WithMessage("Role phải là Admin, Teacher hoặc Student.");
        RuleFor(x => x.StudentId).NotNull()
            .When(x => x.Role == AppRoles.Student)
            .WithMessage("Tài khoản Student phải gắn với một học sinh (StudentId).");
    }
}

public class CreateUserEndpoint(AppDbContext db) : Endpoint<CreateUserRequest, UserDto>
{
    public override void Configure()
    {
        Post("/users");
        Roles(AppRoles.Admin);
        Summary(s => s.Summary = "Tạo tài khoản (chỉ Admin)");
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var username = req.Username.Trim().ToLowerInvariant();

        var exists = await db.Users.IgnoreQueryFilters().AnyAsync(u => u.Username == username, ct);
        if (exists)
        {
            AddError(r => r.Username, ErrorMessages.UsernameExists);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        Guid? studentId = null;
        if (req.Role == AppRoles.Student)
        {
            var studentExists = await db.Students.AnyAsync(s => s.Id == req.StudentId, ct);
            if (!studentExists)
            {
                AddError(r => r.StudentId, ErrorMessages.StudentNotFound);
                await Send.ErrorsAsync(400, ct);
                return;
            }

            var linked = await db.Users.IgnoreQueryFilters()
                .AnyAsync(u => u.StudentId == req.StudentId, ct);
            if (linked)
            {
                AddError(r => r.StudentId, ErrorMessages.StudentAlreadyLinked);
                await Send.ErrorsAsync(400, ct);
                return;
            }

            studentId = req.StudentId;
        }

        var user = new AppUser
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            FullName = req.FullName.Trim(),
            Role = req.Role,
            StudentId = studentId
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        await Send.ResponseAsync(user.ToDto(), 201, ct);
    }
}