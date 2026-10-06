using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.MapperProfiles;
using StudentManagementWeb.Core.Models.Scores;
using StudentManagementWeb.Core.SharedServices;

namespace StudentManagementWeb.Core.Endpoints.Scores;

public class GetStudentTranscriptEndpoint(AppDbContext db)
    : Endpoint<StudentTranscriptRequest, StudentTranscriptDto>
{
    public override void Configure()
    {
        Get("/students/{studentId}/scores");
        
        Summary(s => s.Summary = "Bảng điểm của một học sinh, kèm điểm trung bình");
    }

    public override async Task HandleAsync(StudentTranscriptRequest req, CancellationToken ct)
    {
        var student = await db.Students
            .AsNoTracking()
            .Include(s => s.Class)
            .FirstOrDefaultAsync(s => s.Id == req.StudentId, ct);

        if (student is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var query = db.Scores.AsNoTracking()
            .Include(s => s.Subject)
            .Where(s => s.StudentId == req.StudentId);

        if (req.Semester is not null)
            query = query.Where(s => s.Semester == req.Semester);

        if (!string.IsNullOrWhiteSpace(req.SchoolYear))
        {
            var year = req.SchoolYear.Trim();
            query = query.Where(s => s.SchoolYear == year);
        }

        var scores = await query
            .OrderByDescending(s => s.SchoolYear)
            .ThenBy(s => s.Semester)
            .ThenBy(s => s.Subject.Name)
            .ToListAsync(ct);

        decimal? average = scores.Count == 0 ? null : Math.Round(scores.Average(s => s.Value), 2);

        await Send.OkAsync(new StudentTranscriptDto
        {
            StudentId = student.Id,
            StudentCode = student.Code,
            StudentName = student.FullName,
            ClassName = student.Class?.Name ?? string.Empty,
            Items = scores.Select(s => s.ToTranscriptItem()).ToList(),
            AverageScore = average,
            Classification = average is null ? null : GradeClassifier.Classify(average.Value)
        }, ct);
    }
}