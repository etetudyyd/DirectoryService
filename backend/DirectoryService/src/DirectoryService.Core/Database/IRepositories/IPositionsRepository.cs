using CSharpFunctionalExtensions;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Position;
using Shared.SharedKernel;

namespace DirectoryService.Database.IRepositories;

public interface IPositionsRepository
{
    Task<Guid> AddAsync(Position position, CancellationToken cancellationToken);

    Task<Result<Position, Error>> GetByIdWithLockAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> IsNameUniqueAsync(PositionName name, CancellationToken cancellationToken);

    Task<UnitResult<Error>> BulkDeleteInactivePositionsAsync(List<DepartmentId> departmentIds, CancellationToken cancellationToken);
}