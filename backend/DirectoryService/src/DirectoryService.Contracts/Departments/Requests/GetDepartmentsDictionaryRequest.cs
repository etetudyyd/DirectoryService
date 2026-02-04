namespace DirectoryService.Departments.Requests;

public record GetDepartmentsDictionaryRequest
{
    public string? Search { get; init; }

    public int? Page { get; init; } = PaginationConstants.DEFAULT_PAGE_INDEX;

    public int? PageSize { get; init; } = PaginationConstants.DEFAULT_PAGE_SIZE;
}