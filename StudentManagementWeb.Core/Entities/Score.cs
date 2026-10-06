using StudentManagement.Core.Entities;

namespace StudentManagementWeb.Core.Entities;

public class Score : BaseEntity
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public int Semester { get; set; }

    public string SchoolYear { get; set; } = string.Empty;

    public decimal Value { get; set; }
}