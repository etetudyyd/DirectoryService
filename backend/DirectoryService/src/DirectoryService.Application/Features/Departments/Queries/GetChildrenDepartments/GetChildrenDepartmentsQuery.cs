using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Contracts.Departments.Requests;

namespace DirectoryService.Application.Features.Departments.Queries.GetChildrenDepartments;

public record GetChildrenDepartmentsQuery(GetChildrenDepartmentsRequest Request) : IQuery;