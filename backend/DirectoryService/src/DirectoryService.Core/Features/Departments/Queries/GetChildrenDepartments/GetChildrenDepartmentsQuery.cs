using Core.Abstractions;
using DirectoryService.Departments.Requests;

namespace DirectoryService.Features.Departments.Queries.GetChildrenDepartments;

public record GetChildrenDepartmentsQuery(GetChildrenDepartmentsRequest Request) : IQuery;