namespace DirectoryService.Locations.Requests;

public record GetLocationsRequest
{
    public List<Guid>? DepartmentIds { get; init; } = [];

    public string? Search { get; init; }

    public bool? IsActive { get; init; }

    public int? Page { get; init; } = PaginationConstants.DEFAULT_PAGE;

    public int? PageSize { get; init; } = PaginationConstants.DEFAULT_PAGE_SIZE;
}