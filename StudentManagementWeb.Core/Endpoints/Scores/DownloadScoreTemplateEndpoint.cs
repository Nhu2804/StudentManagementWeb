using FastEndpoints;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.SharedServices;

namespace StudentManagementWeb.Core.Endpoints.Scores;

public class DownloadScoreTemplateEndpoint(IScoreExcelService excel) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/scores/import/template");
        Roles(AppRoles.Admin);   // sau này thêm AppRoles.Teacher
        Summary(s => s.Summary = "Tải file Excel mẫu để nhập điểm");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var bytes = excel.CreateTemplate();
        await Send.BytesAsync(
            bytes,
            ScoreImportConstants.TemplateFileName,
            ScoreImportConstants.XlsxContentType,
            cancellation: ct);
    }
}