using Core.Abstractions;
using DirectoryService.Locations.Requests;

namespace DirectoryService.Features.Locations.Queries.GetLocations;

public record GetLocationsQuery : IQuery
{
    public IReadOnlyList<Guid> Ids { get; }

    public string? Search { get; }

    public bool IsActive { get; }

    public int Page { get; } = PaginationConstants.DEFAULT_PAGE_INDEX;

    public int PageSize { get; } = PaginationConstants.DEFAULT_PAGE_SIZE;

    public GetLocationsQuery(GetLocationsRequest request)
    {
        Ids = request.Ids ?? [];
        Search = request.Search;
        IsActive = request.IsActive ?? true;
        Page = request.Page ?? PaginationConstants.DEFAULT_PAGE_INDEX;
        PageSize = request.PageSize ?? PaginationConstants.DEFAULT_PAGE_SIZE;
    }
}