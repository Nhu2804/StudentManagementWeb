using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Common;
using StudentManagementWeb.Core.Models.Students;

namespace StudentManagementWeb.Core.Endpoints.Students;

public class ListStudentsEndpoint(AppDbContext db, ISieveProcessor sieve)
    : Endpoint<ListStudentsRequest, PagedResult<StudentDto>>
{
    public override void Configure()
    {
        Get("/students");
        Roles(AppRoles.Admin, AppRoles.Teacher);

        Summary(s => s.Summary = "Danh sách học sinh (lọc/sắp xếp bằng Sieve)");
    }

    public override async Task HandleAsync(ListStudentsRequest req, CancellationToken ct)
    {
        // Include(Class) để có tên lớp trả về; Sieve chỉ lọc + sắp xếp, phân trang làm bên dưới.
        var query = sieve.Apply(
            req.ToSieveModel(),
            db.Students.AsNoTracking().Include(s => s.Class),
            applyPagination: false);

        // Client không truyền Sorts thì sắp theo mã học sinh.
        if (string.IsNullOrWhiteSpace(req.Sorts))
            query = query.OrderBy(s => s.Code);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((req.SafePage - 1) * req.SafePageSize)
            .Take(req.SafePageSize)
            .ToListAsync(ct);

        await Send.OkAsync(new PagedResult<StudentDto>
        {
            Items = items.Select(s => s.ToDto()).ToList(),
            Page = req.SafePage,
            PageSize = req.SafePageSize,
            TotalItems = total
        }, ct);
    }
}