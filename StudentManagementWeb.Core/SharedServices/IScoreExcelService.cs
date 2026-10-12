using StudentManagementWeb.Core.Models.Scores;

namespace StudentManagementWeb.Core.SharedServices;

public interface IScoreExcelService
{
    /// <summary>Tạo file mẫu nhập điểm (.xlsx).</summary>
    byte[] CreateTemplate();

    /// <summary>Quét file, trả các dòng có dữ liệu. Ném ExcelFileException nếu file không dùng được.</summary>
    IReadOnlyList<ScannedRow> ReadRows(Stream stream);
}