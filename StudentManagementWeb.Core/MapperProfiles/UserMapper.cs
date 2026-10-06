using StudentManagementWeb.Core.Entities;
using StudentManagementWeb.Core.Models.Auth;

namespace StudentManagementWeb.Core.MapperProfiles;

public static class UserMapper
{
    public static UserDto ToDto(this AppUser u) => new()
    {
        Id = u.Id,
        Username = u.Username,
        FullName = u.FullName
    };
}