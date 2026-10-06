using StudentManagement.Core.Entities;

namespace StudentManagementWeb.Core.Entities;

public class Subject : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<Score> Scores { get; set; } = new List<Score>();
}