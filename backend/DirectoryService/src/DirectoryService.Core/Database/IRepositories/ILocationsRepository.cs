using CSharpFunctionalExtensions;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Location;
using Shared.SharedKernel;

namespace DirectoryService.Database.IRepositories;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> IsNameUniqueAsync(LocationName name, CancellationToken cancellationToken);

    Task<bool> IsAddressExistsAsync(Address address, CancellationToken cancellationToken);

    Task<UnitResult<Error>> IsLocationActiveAsync(LocationId[] locationIds, CancellationToken cancellationToken);

    Task<UnitResult<Error>> BulkDeleteInactiveLocationsAsync(List<DepartmentId> departmentIds, CancellationToken cancellationToken);

    //Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken);

    //Task UpdateAsync(Guid id, Location location, CancellationToken cancellationToken);

    //Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken);

    //Task<Location> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}