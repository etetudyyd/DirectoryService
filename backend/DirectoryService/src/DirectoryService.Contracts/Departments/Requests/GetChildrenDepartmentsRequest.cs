namespace DirectoryService.Departments.Requests;

public record GetChildrenDepartmentsRequest
{
    public GetChildrenDepartmentsRequest(Guid parentId, int? page, int? pageSize)
    {
        ParentId = parentId;
        Page = page ?? PaginationConstants.DEFAULT_PAGE_INDEX;
        PageSize = pageSize ?? PaginationConstants.DEFAULT_PAGE_SIZE;
    }

    public Guid ParentId { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }

}
