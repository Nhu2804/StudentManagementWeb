using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using StudentManagementWeb.Core.Entities;

namespace StudentManagementWeb.Core.Sieve;

public class AppSieveProcessor(IOptions<SieveOptions> options) : SieveProcessor(options)
{
    /// <summary>
    /// Khai báo thuộc tính nào của entity được phép lọc/sắp xếp.
    /// Thuộc tính không khai báo ở đây sẽ bị Sieve bỏ qua.
    /// </summary>
    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        mapper.Property<Subject>(s => s.Code).CanFilter().CanSort();
        mapper.Property<Subject>(s => s.Name).CanFilter().CanSort();
        mapper.Property<Student>(s => s.Code).CanFilter().CanSort();
        mapper.Property<Student>(s => s.FullName).CanFilter().CanSort();
        mapper.Property<Student>(s => s.Email).CanFilter().CanSort();
        mapper.Property<Student>(s => s.Phone).CanFilter();
        mapper.Property<Student>(s => s.Gender).CanFilter().CanSort();
        mapper.Property<Student>(s => s.DateOfBirth).CanSort();
        mapper.Property<Student>(s => s.ClassId).CanFilter();

        // Cột của bảng liên kết: lọc/sắp theo tên lớp, client gọi bằng tên "ClassName"
        mapper.Property<Student>(s => s.Class.Name)
            .CanFilter()
            .CanSort()
            .HasName("ClassName");
        mapper.Property<Class>(c => c.Name).CanFilter().CanSort();
        mapper.Property<Class>(c => c.Grade).CanFilter().CanSort();
        mapper.Property<Class>(c => c.SchoolYear).CanFilter().CanSort();
        mapper.Property<Score>(s => s.Semester).CanFilter().CanSort();
        mapper.Property<Score>(s => s.SchoolYear).CanFilter().CanSort();
        mapper.Property<Score>(s => s.Value).CanFilter().CanSort();

        // Cột của bảng liên kết, client gọi bằng các tên bên dưới
        mapper.Property<Score>(s => s.Student.FullName).CanFilter().CanSort().HasName("StudentName");
        mapper.Property<Score>(s => s.Student.Code).CanFilter().CanSort().HasName("StudentCode");
        mapper.Property<Score>(s => s.Subject.Name).CanFilter().CanSort().HasName("SubjectName");
        mapper.Property<Score>(s => s.Subject.Code).CanFilter().CanSort().HasName("SubjectCode");
        return mapper;
    }
}