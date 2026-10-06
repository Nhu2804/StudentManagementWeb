namespace StudentManagementWeb.Core.Constants;

public static class AppConstants
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public const string StudentCodePrefix = "HS";

    public const decimal MinScore = 0m;
    public const decimal MaxScore = 10m;
}

public static class ErrorMessages
{
    public const string StudentNotFound = "Không tìm thấy học sinh.";
    public const string ClassNotFound = "Không tìm thấy lớp học.";
    public const string ClassNameExists = "Tên lớp đã tồn tại trong năm học này.";
    public const string ClassHasStudents = "Lớp đang có học sinh, không thể xoá.";

    public const string SubjectNotFound = "Không tìm thấy môn học.";
    public const string SubjectCodeExists = "Mã môn học đã tồn tại.";
    public const string SubjectHasScores = "Môn học đã có điểm, không thể xoá.";

    public const string ScoreNotFound = "Không tìm thấy điểm.";
    public const string ScoreExists = "Học sinh đã có điểm môn này trong học kỳ/năm học này.";
    public const string InvalidCredentials = "Tên đăng nhập hoặc mật khẩu không đúng.";
    public const string AccountDisabled = "Tài khoản đã bị khoá.";
}

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
}