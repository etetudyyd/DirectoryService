namespace DirectoryService;

public record PaginationResponse<T>(List<T> Items,
    int TotalItems,
    int Page,
    int PageSize)
{
    public long TotalPages =>
        (long)Math.Ceiling(TotalItems / (double)PageSize);
}