using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.Constants;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.Models.Classes;
using StudentManagementWeb.Core.SharedServices;

namespace StudentManagementWeb.Core.Endpoints.Classes;

public class GetClassRankingEndpoint(AppDbContext db)
    : Endpoint<ClassRankingRequest, ClassRankingDto>
{
    public override void Configure()
    {
        Get("/classes/{classId}/ranking");
        Roles(AppRoles.Admin, AppRoles.Teacher);

        Summary(s => s.Summary =
            "Xếp hạng học sinh trong lớp theo điểm trung bình (lọc theo học kỳ/năm học)");
    }

    public override async Task HandleAsync(ClassRankingRequest req, CancellationToken ct)
    {
        var cls = await db.Classes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == req.ClassId, ct);
        if (cls is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var students = await db.Students.AsNoTracking()
            .Where(s => s.ClassId == req.ClassId)
            .Select(s => new { s.Id, s.Code, s.FullName })
            .ToListAsync(ct);

        var scores = db.Scores.AsNoTracking()
            .Where(s => s.Student.ClassId == req.ClassId);

        if (req.Semester is not null)
            scores = scores.Where(s => s.Semester == req.Semester);

        if (!string.IsNullOrWhiteSpace(req.SchoolYear))
        {
            var year = req.SchoolYear.Trim();
            scores = scores.Where(s => s.SchoolYear == year);
        }

        // Điểm trung bình của từng học sinh, tính ngay trong database.
        var stats = (await scores
                .GroupBy(s => s.StudentId)
                .Select(g => new { StudentId = g.Key, Average = g.Average(x => x.Value), Count = g.Count() })
                .ToListAsync(ct))
            .ToDictionary(x => x.StudentId, x => (x.Average, x.Count));

        var items = students.Select(s =>
        {
            var has = stats.TryGetValue(s.Id, out var st);
            decimal? avg = has ? Math.Round(st.Average, 2) : null;

            return new RankingItemDto
            {
                StudentId = s.Id,
                StudentCode = s.Code,
                StudentName = s.FullName,
                ScoreCount = has ? st.Count : 0,
                AverageScore = avg,
                Classification = avg is null ? null : GradeClassifier.Classify(avg.Value)
            };
        }).ToList();

        // Hạng kiểu thi đấu: bằng điểm thì cùng hạng, hạng kế tiếp bị nhảy (1, 1, 3).
        foreach (var item in items.Where(i => i.AverageScore is not null))
            item.Rank = 1 + items.Count(x => x.AverageScore > item.AverageScore);

        var ordered = items
            .OrderBy(i => i.Rank is null)   // có hạng trước, chưa có điểm xuống cuối
            .ThenBy(i => i.Rank)
            .ThenBy(i => i.StudentCode)
            .ToList();

        await Send.OkAsync(new ClassRankingDto
        {
            ClassId = cls.Id,
            ClassName = cls.Name,
            Items = ordered
        }, ct);
    }
}