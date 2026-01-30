using Core.Abstractions;
using DirectoryService.Positions.Requests;

namespace DirectoryService.Features.Positions.Queries.GetPositions;


public record GetPositionsQuery : IQuery
{
    public IReadOnlyList<Guid> DepartmentIds { get; }

    public string? Search { get; }

    public bool? IsActive { get; }

    public int Page { get; } = PaginationConstants.DEFAULT_PAGE_INDEX;

    public int PageSize { get; } = PaginationConstants.DEFAULT_PAGE_SIZE;

    public GetPositionsQuery(GetPositionsRequest request)
    {
        DepartmentIds = request.DepartmentIds ?? [];
        Search = request.Search;
        IsActive = request.IsActive;
        Page = request.Page ?? PaginationConstants.DEFAULT_PAGE_INDEX;
        PageSize = request.PageSize ?? PaginationConstants.DEFAULT_PAGE_SIZE;
    }
}