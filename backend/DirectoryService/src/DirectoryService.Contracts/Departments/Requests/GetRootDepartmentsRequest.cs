namespace DirectoryService.Departments.Requests;

public record GetRootDepartmentsRequest
{
    public int Page { get; init; }

    public int PageSize { get; init; }

    public int Prefetch { get; init; }

    public GetRootDepartmentsRequest(
        int? page,
        int? pageSize,
        int? prefetch)
    {
        Page = page ?? PaginationConstants.DEFAULT_PAGE_INDEX;
        PageSize = pageSize ?? PaginationConstants.DEFAULT_PAGE_SIZE;
        Prefetch = prefetch ?? PaginationConstants.MAX_PREFETCH_PAGE_INDEX;
    }
}