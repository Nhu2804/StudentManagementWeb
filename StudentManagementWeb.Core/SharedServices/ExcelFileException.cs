namespace StudentManagementWeb.Core.SharedServices;

/// <summary>Lỗi của cả file (sai mẫu, không đọc được, quá nhiều dòng...) => trả 400.</summary>
public class ExcelFileException(string message) : Exception(message);