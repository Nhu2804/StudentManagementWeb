using FastEndpoints;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.SharedServices;

namespace StudentManagementWeb.Core.Endpoints.Scores;

[HttpGet(ApiRoutes.Scores.ImportTemplate)]
public class DownloadScoreTemplateEndpoint(IScoreExcelService excel) : EndpointWithoutRequest
{
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

public class DownloadScoreTemplateSummary : Summary<DownloadScoreTemplateEndpoint>
{
    public DownloadScoreTemplateSummary()
    {
        Summary = "Tải file Excel mẫu để nhập điểm";
    }
}