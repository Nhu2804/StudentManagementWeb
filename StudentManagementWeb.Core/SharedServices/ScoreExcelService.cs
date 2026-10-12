using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.Models.Scores;

namespace StudentManagementWeb.Core.SharedServices;

public class ScoreExcelService : IScoreExcelService
{
    private const int ColumnCount = 5;

    public byte[] CreateTemplate()
    {
        using var wb = new XLWorkbook();

        // Sheet nhập liệu: chỉ có tiêu đề, không có dòng mẫu để khỏi import nhầm.
        var ws = wb.Worksheets.Add(ScoreImportConstants.DataSheetName);
        for (var i = 0; i < ColumnCount; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = ScoreImportConstants.Headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Mã học sinh, mã môn, năm học để dạng Text để Excel không tự đổi kiểu.
        foreach (var col in new[] { 1, 2, 4 })
            ws.Column(col).Style.NumberFormat.Format = "@";

        ws.Columns().AdjustToContents();
        ws.SheetView.FreezeRows(1);

        // Sheet hướng dẫn kèm ví dụ.
        var guide = wb.Worksheets.Add(ScoreImportConstants.GuideSheetName);
        guide.Cell(1, 1).Value = "Hướng dẫn nhập điểm";
        guide.Cell(1, 1).Style.Font.Bold = true;
        guide.Cell(2, 1).Value = "- Điền dữ liệu ở sheet 'Diem', bắt đầu từ dòng 2. Không đổi tiêu đề cột.";
        guide.Cell(3, 1).Value = "- Học kỳ: 1 hoặc 2. Năm học dạng 2026-2027. Điểm từ 0 đến 10 (nhập 8,5 hoặc 8.5 đều được).";
        guide.Cell(4, 1).Value = "- Tối đa 2.000 dòng, file .xlsx dưới 5 MB.";
        guide.Cell(6, 1).Value = "Ví dụ:";
        for (var i = 0; i < ColumnCount; i++)
        {
            guide.Cell(7, i + 1).Value = ScoreImportConstants.Headers[i];
            guide.Cell(7, i + 1).Style.Font.Bold = true;
        }
        guide.Cell(8, 1).Value = "HS20260001";
        guide.Cell(8, 2).Value = "TOAN";
        guide.Cell(8, 3).Value = 1;
        guide.Cell(8, 4).Value = "2026-2027";
        guide.Cell(8, 5).Value = 8.5;
        guide.Column(1).AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public IReadOnlyList<ScannedRow> ReadRows(Stream stream)
    {
        XLWorkbook wb;
        try
        {
            wb = new XLWorkbook(stream);
        }
        catch (Exception)
        {
            throw new ExcelFileException(ErrorMessages.ImportCannotRead);
        }

        using (wb)
        {
            // Ưu tiên sheet "Diem"; không có thì lấy sheet đầu tiên.
            IXLWorksheet? ws;
            if (!wb.Worksheets.TryGetWorksheet(ScoreImportConstants.DataSheetName, out ws))
                ws = wb.Worksheets.FirstOrDefault();

            if (ws is null)
                throw new ExcelFileException(ErrorMessages.ImportCannotRead);

            // Kiểm tra tiêu đề đúng file mẫu.
            for (var i = 0; i < ColumnCount; i++)
            {
                var actual = Normalize(ws.Cell(1, i + 1).GetString());
                var expected = Normalize(ScoreImportConstants.Headers[i]);
                if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
                    throw new ExcelFileException(ErrorMessages.ImportWrongTemplate);
            }

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            var rows = new List<ScannedRow>();

            for (var r = 2; r <= lastRow; r++)
            {
                var raw = new ImportScoreRaw
                {
                    StudentCode = ReadCell(ws.Cell(r, 1)),
                    SubjectCode = ReadCell(ws.Cell(r, 2)),
                    Semester = ReadCell(ws.Cell(r, 3)),
                    SchoolYear = ReadCell(ws.Cell(r, 4)),
                    Score = ReadCell(ws.Cell(r, 5))
                };

                // Dòng trống hoàn toàn thì bỏ qua, không tính là dòng dữ liệu.
                if (raw.StudentCode.Length == 0 && raw.SubjectCode.Length == 0 &&
                    raw.Semester.Length == 0 && raw.SchoolYear.Length == 0 &&
                    raw.Score.Length == 0)
                    continue;

                rows.Add(new ScannedRow(r, raw));

                if (rows.Count > ScoreImportConstants.MaxRows)
                    throw new ExcelFileException(ErrorMessages.ImportTooManyRows);
            }

            return rows;
        }
    }

    /// <summary>Ô số và ô chữ đều đọc ra chuỗi; việc hiểu dấu phẩy 8,5 để bước sau lo.</summary>
    private static string ReadCell(IXLCell cell)
    {
        if (cell.IsEmpty()) return string.Empty;

        return cell.DataType == XLDataType.Number
            ? cell.GetDouble().ToString("G15", CultureInfo.InvariantCulture)
            : cell.GetString().Trim();
    }

    private static string Normalize(string s) => s.Trim().Normalize(NormalizationForm.FormC);
}