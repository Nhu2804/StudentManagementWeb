using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Auth;

namespace StudentManagementWeb.Core.Endpoints.Auth;

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginEndpoint(AppDbContext db, IConfiguration config)
    : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
        Summary(s => s.Summary = "Đăng nhập, trả về JWT");
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var username = req.Username.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
        {
            AddError(r => r.Password, ErrorMessages.InvalidCredentials);
            await Send.ErrorsAsync(401, ct);
            return;
        }

        if (!user.IsActive)
        {
            AddError(r => r.Username, ErrorMessages.AccountDisabled);
            await Send.ErrorsAsync(403, ct);
            return;
        }

        var signingKey = config["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Thiếu Jwt:SigningKey trong cấu hình.");
        var expireMinutes = int.TryParse(config["Jwt:ExpireMinutes"], out var m) ? m : 120;
        var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

        var token = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = signingKey;
            o.ExpireAt = expiresAt;
            o.User.Claims.Add(("UserId", user.Id.ToString()));
            o.User.Claims.Add(("Username", user.Username));
        });

        await Send.OkAsync(new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = user.ToDto()
        }, ct);
    }
}