using FastEndpoints;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.Models.Scores;
using StudentManagementWeb.Core.SharedServices;

namespace StudentManagementWeb.Core.Endpoints.Scores;

public class ImportScoresEndpoint(IScoreImportService importService)
    : Endpoint<ScoreImportRequest, ScoreImportResult>
{
    public override void Configure()
    {
        Post("/scores/import");
        AllowFileUploads();
        Roles(AppRoles.Admin);   // sau này thêm AppRoles.Teacher + kiểm tra lớp của giáo viên
        Summary(s => s.Summary =
            "Import điểm từ file Excel (.xlsx, tối đa 5 MB và 2.000 dòng)");
    }

    public override async Task HandleAsync(ScoreImportRequest req, CancellationToken ct)
    {
        var file = req.File;

        if (file is null)
        {
            AddError(r => r.File, ErrorMessages.ImportFileRequired);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        if (!string.Equals(Path.GetExtension(file.FileName), ScoreImportConstants.XlsxExtension,
                StringComparison.OrdinalIgnoreCase))
        {
            AddError(r => r.File, ErrorMessages.ImportInvalidExtension);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        if (file.Length == 0)
        {
            AddError(r => r.File, ErrorMessages.ImportFileEmpty);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        if (file.Length > ScoreImportConstants.MaxFileSizeBytes)
        {
            AddError(r => r.File, ErrorMessages.ImportFileTooLarge);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        ScoreImportResult result;
        try
        {
            // Chép ra MemoryStream vì thư viện Excel cần stream đọc lùi được.
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            ms.Position = 0;

            result = await importService.ImportAsync(ms, ct);
        }
        catch (ExcelFileException ex)
        {
            AddError(r => r.File, ex.Message);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        // Có ít nhất 1 dòng được ghi thì 200; mọi dòng đều lỗi thì 400 kèm danh sách lỗi.
        if (result.Created + result.Updated == 0)
        {
            await Send.ResponseAsync(result, 400, ct);
            return;
        }

        await Send.OkAsync(result, ct);
    }
}