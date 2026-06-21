using Core.Abstractions;

namespace DirectoryService.Features.Departments.Queries.GetDepartment;

public record GetDepartmentQuery(Guid DepartmentId) : IQuery;