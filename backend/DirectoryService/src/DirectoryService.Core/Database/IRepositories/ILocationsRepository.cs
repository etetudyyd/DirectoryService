using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Location;
using Shared.SharedKernel;

namespace DirectoryService.Database.IRepositories;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetBy(
        Expression<Func<Location, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<Result<Location, Error>> GetWithDepartmentsBy(
        Expression<Func<Location, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<bool> IsNameUniqueAsync(LocationName name, CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByIdWithLockAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> IsAddressExistsAsync(Address address, CancellationToken cancellationToken);


    Task<Result<List<Guid>, Error>> GetActiveLocationIdsAsync(
        Guid[] locationIds,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> BulkDeleteInactiveLocationsAsync(List<DepartmentId> departmentIds, CancellationToken cancellationToken);
    //Task<UnitResult<Error>> IsLocationsActiveAsync(Guid[] locationIds, CancellationToken cancellationToken);

    //Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken);

    //Task UpdateAsync(Guid id, Location location, CancellationToken cancellationToken);

    //Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken);

    //Task<Location> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}