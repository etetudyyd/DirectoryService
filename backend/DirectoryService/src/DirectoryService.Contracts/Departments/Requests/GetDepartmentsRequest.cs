namespace DirectoryService.Departments.Requests;

public record GetDepartmentsRequest
{
    public List<Guid>? LocationsIds { get; init; } = [];

    public Guid? ParentId { get; init; }

    public string? Search { get; init; } = string.Empty;

    public bool? IsActive { get; init; }

    public int? Page { get; init; } = PaginationConstants.DEFAULT_PAGE;

    public int? PageSize { get; init; } = PaginationConstants.DEFAULT_PAGE_SIZE;

    public string SortBy { get; init; } = string.Empty;

    public string SortDirection { get; init; } = "asc";
}