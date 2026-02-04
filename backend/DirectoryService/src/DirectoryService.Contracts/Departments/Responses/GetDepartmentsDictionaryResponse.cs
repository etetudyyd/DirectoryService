namespace DirectoryService.Departments.Responses;

public record GetDepartmentsDictionaryResponse(
    List<DictionaryItemResponse> Items,
    int TotalItems,
    int Page,
    int PageSize)
{
public long TotalPages =>
    (long)Math.Ceiling(TotalItems / (double)PageSize);
}