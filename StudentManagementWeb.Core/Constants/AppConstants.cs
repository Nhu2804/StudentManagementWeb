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
    public const string UsernameExists = "Tên đăng nhập đã tồn tại.";
    public const string StudentAlreadyLinked = "Học sinh này đã có tài khoản.";

    public const string ImportFileRequired = "Vui lòng chọn file Excel.";
    public const string ImportFileEmpty = "File rỗng.";
    public const string ImportFileTooLarge = "File vượt quá 5 MB.";
    public const string ImportInvalidExtension = "Chỉ nhận file .xlsx.";
    public const string ImportCannotRead = "Không đọc được file. Hãy dùng đúng file mẫu (.xlsx).";
    public const string ImportWrongTemplate = "File không đúng mẫu: tiêu đề cột không khớp.";
    public const string ImportNoData = "File không có dòng dữ liệu nào.";
    public const string ImportTooManyRows = "File vượt quá 2.000 dòng dữ liệu.";

    public const string ImportRowMissing = "Thiếu dữ liệu ở cột: {0}.";
    public const string ImportInvalidSemester = "Học kỳ phải là 1 hoặc 2.";
    public const string ImportInvalidSchoolYear = "Năm học phải có dạng 2026-2027.";
    public const string ImportScoreNotNumber = "Điểm không phải là số.";
    public const string ImportScoreOutOfRange = "Điểm phải từ 0 đến 10.";
    public const string ImportStudentNotFound = "Không có học sinh mã {0}.";
    public const string ImportSubjectNotFound = "Không có môn học mã {0}.";
    public const string ImportDuplicateRow = "Trùng với dòng {0} trong file (cùng học sinh, môn, học kỳ, năm học).";
}

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Student = "Student";

    public static readonly string[] All = [Admin, Teacher, Student];
}

public static class ScoreImportConstants
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;   // 5 MB
    public const int MaxRows = 2000;

    public const string XlsxExtension = ".xlsx";
    public const string XlsxContentType =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public const string TemplateFileName = "mau-nhap-diem.xlsx";
    public const string DataSheetName = "Diem";
    public const string GuideSheetName = "Huong dan";

    /// <summary>Tiêu đề 5 cột, theo đúng thứ tự trong file mẫu.</summary>
    public static readonly string[] Headers =
        ["Mã học sinh", "Mã môn", "Học kỳ", "Năm học", "Điểm"];
}