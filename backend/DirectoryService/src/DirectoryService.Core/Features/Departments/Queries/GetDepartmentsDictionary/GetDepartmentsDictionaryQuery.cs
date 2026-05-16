using Core.Abstractions;
using DirectoryService.Departments.Requests;

namespace DirectoryService.Features.Departments.Queries.GetDepartmentsDictionary;

public record GetDepartmentsDictionaryQuery(GetDepartmentsDictionaryRequest DepartmentsRequest) : IQuery;