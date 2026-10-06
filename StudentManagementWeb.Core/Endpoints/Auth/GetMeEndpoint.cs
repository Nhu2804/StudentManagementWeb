using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Auth;

namespace StudentManagementWeb.Core.Endpoints.Auth;

public class GetMeEndpoint(AppDbContext db) : EndpointWithoutRequest<UserDto>
{
    public override void Configure()
    {
        Get("/auth/me");
        Summary(s => s.Summary = "Thông tin tài khoản đang đăng nhập");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirst("UserId")?.Value, out var userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        await Send.OkAsync(user.ToDto(), ct);
    }
}