using StudentManagement.Core.Entities;

namespace StudentManagementWeb.Core.Entities;

public class Class : BaseEntity
{

    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string SchoolYear { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}