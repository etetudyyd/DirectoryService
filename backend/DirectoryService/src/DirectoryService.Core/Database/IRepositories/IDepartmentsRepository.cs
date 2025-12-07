using Core.Options;
using CSharpFunctionalExtensions;
using DirectoryService.Entities;
using DirectoryService.Features.Departments.Commands.DeleteInactiveDepartments;
using DirectoryService.ValueObjects.Department;
using Shared.SharedKernel;
using Path = DirectoryService.ValueObjects.Department.Path;

namespace DirectoryService.Database.IRepositories;

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

    Task<Result<List<Department>, Error>> GetAllInactiveDepartmentsAsync(TimeOptions timeOptions, CancellationToken cancellationToken);

    Task<Result<List<Department>, Error>> GetChildrenDepartmentsAsync(List<DepartmentId> ids, CancellationToken cancellationToken);

    Task<Result<List<Department>, Error>> GetParentDepartmentsAsync(List<DepartmentId> ids, CancellationToken cancellationToken);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteDepartmentsAsync(List<DepartmentId> departmentIds, CancellationToken cancellationToken);

    Task<UnitResult<Error>> BulkUpdateDescendantsPathAsync(List<UpdatePath> moves, CancellationToken cancellationToken);
}