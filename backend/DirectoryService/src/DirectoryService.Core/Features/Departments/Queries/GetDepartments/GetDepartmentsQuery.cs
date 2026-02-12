using Core.Abstractions;
using DirectoryService.Departments.Requests;

namespace DirectoryService.Features.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery(GetDepartmentsDictionaryRequest DepartmentsDictionaryRequest) : IQuery;