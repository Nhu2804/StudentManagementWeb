using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Models.Classes;

public class ClassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string SchoolYear { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}

public class CreateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string SchoolYear { get; set; } = string.Empty;
}

public class ListClassesRequest : SieveRequest
{
    public int? Grade { get; set; }
}

public class UpdateClassRequest : CreateClassRequest
{
    public Guid Id { get; set; }
}