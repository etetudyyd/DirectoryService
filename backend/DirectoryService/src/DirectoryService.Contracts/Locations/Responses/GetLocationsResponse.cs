namespace DirectoryService.Locations.Responses;

public record GetLocationsResponse(
    List<LocationDto> Items,
    int TotalItems,
    int Page,
    int PageSize)
{
    public long TotalPages =>
        (long)Math.Ceiling(TotalItems / (double)PageSize);
}