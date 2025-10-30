using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
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

    Task<Result<Guid, Error>> Delete(Department department, CancellationToken cancellationToken);
}