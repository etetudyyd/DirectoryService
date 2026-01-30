namespace DirectoryService.Positions.Responses;

public record GetPositionsResponse(
    List<PositionDto> Items,
    int TotalItems,
    int Page,
    int PageSize)
{
    public long TotalPages =>
        (long)Math.Ceiling(TotalItems / (double)PageSize);
}