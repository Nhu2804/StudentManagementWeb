namespace StudentManagementWeb.Core.Constants;

public static class EndpointPermissions
{
    public static readonly IReadOnlyList<(string RoutePrefix, string[] Roles)> Rules =
    [
        (ApiRoutes.Scores.Import, [AppRoles.Admin]),
    ];
}