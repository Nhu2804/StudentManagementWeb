using StudentManagementWeb.Core.Constants;

namespace StudentManagementWeb.Middlewares;

public class AuthorizationMiddleware(RequestDelegate next)
{
    private const string ApiPrefix = "/api";

    public async Task Invoke(HttpContext context)
    {
        foreach (var (routePrefix, roles) in EndpointPermissions.Rules)
        {
            if (!context.Request.Path.StartsWithSegments($"{ApiPrefix}{routePrefix}"))
                continue;

            // Chưa đăng nhập (hoặc token hết hạn, sai) => 401.
            if (context.User.Identity?.IsAuthenticated != true)
            {
                await WriteErrorAsync(context, StatusCodes.Status401Unauthorized, "Chưa đăng nhập.");
                return;
            }

            // Đã đăng nhập nhưng không có vai trò được phép => 403.
            if (!roles.Any(context.User.IsInRole))
            {
                await WriteErrorAsync(context, StatusCodes.Status403Forbidden, "Bạn không có quyền thực hiện thao tác này.");
                return;
            }
        }

        await next(context);
    }

    private static Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new { statusCode, message });
    }
}