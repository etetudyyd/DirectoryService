namespace DirectoryService.Departments.Responses;

public record GetDepartmentsResponse(
    List<DepartmentItemDto> Items,
    int TotalItems,
    int Page,
    int PageSize)
{
public long TotalPages =>
    (long)Math.Ceiling(TotalItems / (double)PageSize);
}