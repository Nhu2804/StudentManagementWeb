using Sieve.Models;
using StudentManagementWeb.Core.Models.Common;

namespace StudentManagementWeb.Core.Models.Subjects;

public class SubjectDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class CreateSubjectRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class UpdateSubjectRequest : CreateSubjectRequest
{
    public Guid Id { get; set; }   // lấy từ route /subjects/{id}
}

public class ListSubjectsRequest : SieveRequest;