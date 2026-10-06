using Sieve.Models;
using StudentManagementWeb.Core.Constants;

namespace StudentManagementWeb.Core.Models.Common;

/// <summary>
/// Request lọc/sắp xếp/phân trang dùng chung cho các endpoint danh sách.
/// Thuộc tính nullable để Swagger không đánh dấu "required".
/// </summary>
public class SieveRequest
{
    public string? Filters { get; set; }
    public string? Sorts { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public int SafePage => Page is > 0 ? Page.Value : 1;

    public int SafePageSize => PageSize is > 0
        ? Math.Min(PageSize.Value, AppConstants.MaxPageSize)
        : AppConstants.DefaultPageSize;
    public SieveModel ToSieveModel() => new()
    {
        Filters = Filters,
        Sorts = Sorts,
        Page = Page,
        PageSize = PageSize
    };
}