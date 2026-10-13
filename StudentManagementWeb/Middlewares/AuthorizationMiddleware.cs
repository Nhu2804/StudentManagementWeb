using System.Text.RegularExpressions;
using StudentManagementWeb.Core.Constants;

namespace StudentManagementWeb.Middlewares;

public class AuthorizationMiddleware(RequestDelegate next)
{
    private const string ApiPrefix = "/api";

    // Biên dịch regex một lần cho mỗi luật (giữ nguyên thứ tự của EndpointPermissions.Rules).
    private static readonly IReadOnlyList<(PermissionRule Rule, Regex Regex)> Compiled =
        EndpointPermissions.Rules
            .Select(r => (r, ToRegex(r.RoutePrefix)))
            .ToList();

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(ApiPrefix, out var rest))
        {
            var path = rest.Value ?? string.Empty;

            // Luật ĐẦU TIÊN khớp phương thức và đường dẫn sẽ quyết định.
            var match = Compiled.FirstOrDefault(c =>
                (c.Rule.Method is null
                    || string.Equals(c.Rule.Method, context.Request.Method, StringComparison.OrdinalIgnoreCase))
                && c.Regex.IsMatch(path));

            if (match.Rule is not null)
            {
                // Chưa đăng nhập (hoặc token hết hạn, sai) => 401.
                if (context.User.Identity?.IsAuthenticated != true)
                {
                    await WriteErrorAsync(context, StatusCodes.Status401Unauthorized, "Chưa đăng nhập.");
                    return;
                }

                // Đã đăng nhập nhưng không có vai trò được phép => 403.
                if (!match.Rule.Roles.Any(context.User.IsInRole))
                {
                    await WriteErrorAsync(context, StatusCodes.Status403Forbidden, "Bạn không có quyền thực hiện thao tác này.");
                    return;
                }
            }
        }

        await next(context);
    }

    /// <summary>
    /// "/students/{studentId}/scores" -> ^/students/[^/]+/scores(/.*)?$
    /// Mỗi {tham số} khớp đúng một đoạn đường dẫn; phần sau cùng khớp theo tiền tố.
    /// </summary>
    private static Regex ToRegex(string routeTemplate)
    {
        var segments = routeTemplate.Trim('/').Split('/')
            .Select(s => s.StartsWith('{') ? "[^/]+" : Regex.Escape(s));

        var pattern = "^/" + string.Join("/", segments) + "(/.*)?$";
        return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private static Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new { statusCode, message });
    }
}