using CSharpFunctionalExtensions;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.Department;
using Shared.SharedKernel;

namespace DirectoryService.Database.IRepositories;

public interface IPositionsRepository
{
    Task<Guid> AddAsync(Position position, CancellationToken cancellationToken);

    Task<UnitResult<Error>> BulkDeleteInactivePositionsAsync(List<DepartmentId> departmentIds, CancellationToken cancellationToken);
}