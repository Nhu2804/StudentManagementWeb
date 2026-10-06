using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Classes;
using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Endpoints.Classes;

public class ListClassesEndpoint(AppDbContext db, ISieveProcessor sieve)
    : Endpoint<ListClassesRequest, PagedResult<ClassDto>>
{
    public override void Configure()
    {
        Get("/classes");
        Roles(AppRoles.Admin, AppRoles.Teacher);

        Summary(s => s.Summary = "Danh sách lớp học (lọc theo khối + lọc/sắp xếp bằng Sieve)");
    }

    public override async Task HandleAsync(ListClassesRequest req, CancellationToken ct)
    {
        var source = db.Classes.AsNoTracking().AsQueryable();

        if (req.Grade is not null)
            source = source.Where(c => c.Grade == req.Grade);

        var query = sieve.Apply(req.ToSieveModel(), source, applyPagination: false);

        // Client không truyền Sorts thì sắp theo năm học mới nhất, rồi tên lớp.
        if (string.IsNullOrWhiteSpace(req.Sorts))
            query = query.OrderByDescending(c => c.SchoolYear).ThenBy(c => c.Name);

        var total = await query.CountAsync(ct);

        var rows = await query
            .Skip((req.SafePage - 1) * req.SafePageSize)
            .Take(req.SafePageSize)
            .Select(c => new { Class = c, StudentCount = c.Students.Count })
            .ToListAsync(ct);

        await Send.OkAsync(new PagedResult<ClassDto>
        {
            Items = rows.Select(r => r.Class.ToDto(r.StudentCount)).ToList(),
            Page = req.SafePage,
            PageSize = req.SafePageSize,
            TotalItems = total
        }, ct);
    }
}