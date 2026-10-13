using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Common;
using StudentManagementWeb.Core.Models.Scores;

namespace StudentManagementWeb.Core.Endpoints.Scores;

[HttpGet(ApiRoutes.Scores.GetAll)]
public class ListScoresEndpoint(AppDbContext db, ISieveProcessor sieve)
    : Endpoint<ListScoresRequest, PagedResult<ScoreDto>>
{
    public override async Task HandleAsync(ListScoresRequest req, CancellationToken ct)
    {
        var source = db.Scores.AsNoTracking()
            .Include(s => s.Student)
            .Include(s => s.Subject)
            .AsQueryable();

        if (req.StudentId is not null)
            source = source.Where(s => s.StudentId == req.StudentId);

        if (req.SubjectId is not null)
            source = source.Where(s => s.SubjectId == req.SubjectId);

        if (req.ClassId is not null)
            source = source.Where(s => s.Student.ClassId == req.ClassId);

        var query = sieve.Apply(req.ToSieveModel(), source, applyPagination: false);

        // Chưa có sắp xếp nào (không truyền Sorts, hoặc Sieve bỏ qua trường sai) thì dùng thứ tự mặc định.
        if (!query.Expression.ToString().Contains("OrderBy"))
        {
            query = query
                .OrderByDescending(s => s.SchoolYear)
                .ThenBy(s => s.Semester)
                .ThenBy(s => s.Student.Code)
                .ThenBy(s => s.Subject.Code);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((req.SafePage - 1) * req.SafePageSize)
            .Take(req.SafePageSize)
            .ToListAsync(ct);

        await Send.OkAsync(new PagedResult<ScoreDto>
        {
            Items = items.Select(s => s.ToDto()).ToList(),
            Page = req.SafePage,
            PageSize = req.SafePageSize,
            TotalItems = total
        }, ct);
    }
}

public class ListScoresSummary : Summary<ListScoresEndpoint>
{
    public ListScoresSummary()
    {
        Summary = "Danh sách điểm (lọc theo HS/môn/lớp + lọc/sắp xếp bằng Sieve)";
    }
}