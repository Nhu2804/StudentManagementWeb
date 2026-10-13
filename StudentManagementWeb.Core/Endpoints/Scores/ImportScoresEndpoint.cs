using FastEndpoints;
using FluentValidation;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.Models.Scores;
using StudentManagementWeb.Core.SharedServices;

namespace StudentManagementWeb.Core.Endpoints.Scores;

public class ScoreImportRequestValidator : Validator<ScoreImportRequest>
{
    public ScoreImportRequestValidator()
    {
        RuleFor(x => x.File)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ErrorMessages.ImportFileRequired)
            .Must(f => string.Equals(Path.GetExtension(f.FileName),
                    ScoreImportConstants.XlsxExtension, StringComparison.OrdinalIgnoreCase))
                .WithMessage(ErrorMessages.ImportInvalidExtension)
            .Must(f => f.Length > 0)
                .WithMessage(ErrorMessages.ImportFileEmpty)
            .Must(f => f.Length <= ScoreImportConstants.MaxFileSizeBytes)
                .WithMessage(ErrorMessages.ImportFileTooLarge);
    }
}

[HttpPost(ApiRoutes.Scores.Import)]   
[AllowFileUploads]
public class ImportScoresEndpoint(IScoreImportService importService)
    : Endpoint<ScoreImportRequest, ScoreImportResult>
{
    public override async Task HandleAsync(ScoreImportRequest req, CancellationToken ct)
    {
        ScoreImportResult result;
        try
        {
            await using var ms = new MemoryStream();
            await req.File.CopyToAsync(ms, ct);
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

public class ImportScoresSummary : Summary<ImportScoresEndpoint>
{
    public ImportScoresSummary()
    {
        Summary = "Import điểm từ file Excel (.xlsx, tối đa 5 MB và 2.000 dòng)";
    }
}