using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Contracts.Departments.Requests;

namespace DirectoryService.Application.Features.Departments.Queries.GetRootDepartments;

public record GetRootDepartmentsQuery(GetRootDepartmentsRequest DepartmentsRequest) : IQuery;