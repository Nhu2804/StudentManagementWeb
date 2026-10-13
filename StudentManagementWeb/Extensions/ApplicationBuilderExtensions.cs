using StudentManagementWeb.Middlewares;

namespace StudentManagementWeb.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<AuthorizationMiddleware>();
}