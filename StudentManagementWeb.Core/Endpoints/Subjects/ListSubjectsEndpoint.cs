using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Common;
using StudentManagementWeb.Core.Models.Subjects;

namespace StudentManagementWeb.Core.Endpoints.Subjects;

public class ListSubjectsEndpoint(AppDbContext db, ISieveProcessor sieve)
    : Endpoint<ListSubjectsRequest, PagedResult<SubjectDto>>
{
    public override void Configure()
    {
        Get("/subjects");
        
        Summary(s => s.Summary = "Danh sách môn học (lọc/sắp xếp bằng Sieve)");
    }

    public override async Task HandleAsync(ListSubjectsRequest req, CancellationToken ct)
    {
        // Sieve chỉ lọc + sắp xếp; phân trang tự làm bên dưới để trả đúng Page/PageSize thực tế.
        var query = sieve.Apply(req.ToSieveModel(), db.Subjects.AsNoTracking(), applyPagination: false);

        // Client không truyền Sorts thì sắp theo mã môn.
        if (string.IsNullOrWhiteSpace(req.Sorts))
            query = query.OrderBy(s => s.Code);

        var total = await query.CountAsync(ct);

        var page = req.Page is > 0 ? req.Page.Value : 1;
        var pageSize = req.PageSize is > 0
            ? Math.Min(req.PageSize.Value, AppConstants.MaxPageSize)
            : AppConstants.DefaultPageSize;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        await Send.OkAsync(new PagedResult<SubjectDto>
        {
            Items = items.Select(s => s.ToDto()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        }, ct);
    }
}