namespace StudentManagementWeb.Core.Constants;

public record PermissionRule(string? Method, string RoutePrefix, string[] Roles);

public static class EndpointPermissions
{
    public static readonly IReadOnlyList<PermissionRule> Rules =
    [
        new(null, ApiRoutes.Users.Create, [AppRoles.Admin]),

        new(null, ApiRoutes.Scores.Import, [AppRoles.Admin]),
        new("GET", ApiRoutes.Scores.Transcript, [AppRoles.Admin, AppRoles.Teacher, AppRoles.Student]),
        new("GET", ApiRoutes.Scores.Me, [AppRoles.Student]),

        new("POST", ApiRoutes.Classes.Create, [AppRoles.Admin]),
        new("DELETE", ApiRoutes.Classes.Create, [AppRoles.Admin]),
        new("GET", ApiRoutes.Classes.Create, [AppRoles.Admin, AppRoles.Teacher]),
        new("PUT", ApiRoutes.Classes.Create, [AppRoles.Admin]),

        new("POST", ApiRoutes.Students.Create, [AppRoles.Admin]),
        new("DELETE", ApiRoutes.Students.Create, [AppRoles.Admin]),
        new("GET", ApiRoutes.Students.Create, [AppRoles.Admin, AppRoles.Teacher]),
        new("PUT", ApiRoutes.Students.Create, [AppRoles.Admin]),

        new("POST", ApiRoutes.Subjects.Create, [AppRoles.Admin]),
        new("DELETE", ApiRoutes.Subjects.Create, [AppRoles.Admin]),
        new("PUT", ApiRoutes.Subjects.Create, [AppRoles.Admin]),

        new("GET", ApiRoutes.Scores.GetAll, [AppRoles.Admin, AppRoles.Teacher]),
        new("POST", ApiRoutes.Scores.Create, [AppRoles.Admin, AppRoles.Teacher]),
        new("PUT", ApiRoutes.Scores.Create, [AppRoles.Admin, AppRoles.Teacher]),
        new("DELETE", ApiRoutes.Scores.Create, [AppRoles.Admin, AppRoles.Teacher]),
    ];
}