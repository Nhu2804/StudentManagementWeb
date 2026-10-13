namespace StudentManagementWeb.Core.Constants;

public static class ApiRoutes
{
    public static class Auth
    {
        public const string Login = "/auth/login";
        public const string Me = "/auth/me";
    }
    public static class Users
    {
        public const string Create = "/users";
    }

    public static class Classes
    {
        public const string Create = "/classes";
        public const string Delete = "/classes/{id}";
        public const string GetById = "/classes/{id}";
        public const string Ranking = "/classes/{classId}/ranking";
        public const string GetAll = "/classes";
        public const string Update = "/classes/{id}";
    }

    public static class Students
    {
        public const string Create = "/students";
        public const string Delete = "/students/{id}";
        public const string GetById = "/students/{id}";
        public const string GetAll = "/students";
        public const string Update = "/students/{id}";
    }
    public static class Subjects
    {
        public const string Create = "/subjects";
        public const string Delete = "/subjects/{id}";
        public const string GetById = "/subjects/{id}";
        public const string GetAll = "/subjects";
        public const string Update = "/subjects/{id}";
    }
    public static class Scores
    {
        public const string Import = "/scores/import";
        public const string ImportTemplate = "/scores/import/template";
        public const string Create = "/scores";
        public const string Delete = "/scores/{id}";
        public const string Transcript = "/students/{studentId}/scores";
        public const string GetAll = "/scores";
        public const string Update = "/scores/{id}";
        public const string Me = "/scores/me";
    }
}