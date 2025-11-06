using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using Path = DevQuestions.Domain.ValueObjects.DepartmentVO.Path;

namespace DirectoryService.Application.Database.IRepositories;

public interface IDepartmentsRepository
{
    Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdWithLockAsync(Guid id, CancellationToken cancellationToken);

    Task<UnitResult<Error>> LockDescendantsAsync(Department department, CancellationToken cancellationToken);

    Task<bool> IsDescendantAsync(Path rootPath, Guid candidateForCheckId, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> RelocateDepartmentAsync(Department departmentUpdated, Path oldPath, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> UpdateChildDepartmentsPath(Department parent, CancellationToken cancellationToken);

    Task<Result<Guid[], Error>> DeactivateConnectedLocations(DepartmentId departmentId, CancellationToken cancellationToken);

    Task<Result<Guid[], Error>> DeactivateConnectedPositions(DepartmentId departmentId, CancellationToken cancellationToken);
}