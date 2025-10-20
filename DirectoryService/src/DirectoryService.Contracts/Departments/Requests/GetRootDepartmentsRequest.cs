namespace DirectoryService.Contracts.Departments.Requests;

public record GetRootDepartmentsRequest
{
    public int Page { get; init; } = PaginationConstants.DEFAULT_PAGE_INDEX;

    public int PageSize { get; init; } = PaginationConstants.DEFAULT_PAGE_SIZE;

    public int Prefetch { get; init; } = PaginationConstants.MAX_PREFETCH_PAGE_INDEX;
}