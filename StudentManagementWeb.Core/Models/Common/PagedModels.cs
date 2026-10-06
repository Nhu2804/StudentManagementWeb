using StudentManagementWeb.Core.Constants;

namespace StudentManagementWeb.Core.Models.Common;

public class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = AppConstants.DefaultPageSize;
    public string? Search { get; set; }

    public int SafePage => Page < 1 ? 1 : Page;

    public int SafePageSize => PageSize < 1
        ? AppConstants.DefaultPageSize
        : Math.Min(PageSize, AppConstants.MaxPageSize);
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
}

public class IdRequest
{
    public Guid Id { get; set; }
}