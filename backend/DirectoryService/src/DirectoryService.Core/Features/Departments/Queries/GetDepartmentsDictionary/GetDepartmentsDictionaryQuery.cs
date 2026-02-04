using Core.Abstractions;
using DirectoryService.Departments.Requests;

namespace DirectoryService.Features.Departments.Queries.GetDepartmentsDictionary;

public record GetDepartmentsDictionaryQuery : IQuery
{
    public string? Search { get; }

    public int Page { get; } = PaginationConstants.DEFAULT_PAGE_INDEX;

    public int PageSize { get; } = PaginationConstants.DEFAULT_PAGE_SIZE;

    public GetDepartmentsDictionaryQuery(GetDepartmentsDictionaryRequest request)
    {
        Search = request.Search;
        Page = request.Page ?? PaginationConstants.DEFAULT_PAGE_INDEX;
        PageSize = request.PageSize ?? PaginationConstants.DEFAULT_PAGE_SIZE;
    }
}