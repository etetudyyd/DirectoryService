using Core.Abstractions;
using DirectoryService.Departments.Requests;

namespace DirectoryService.Features.Departments.Queries.GetRootDepartments;

public record GetRootDepartmentsQuery(GetRootDepartmentsRequest Request) : IQuery;