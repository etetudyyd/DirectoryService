using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database;
using DirectoryService.Database.IRepositories;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Location;
using Microsoft.EntityFrameworkCore;
using Shared.SharedKernel;

namespace DirectoryService.Repositories;

/// <summary>
/// LocationsRepository - realization of Repository Pattern for Locations database logic by using DirectoryServiceDbContext.
/// It realizes interface ILocationsRepository.
/// </summary>
public class LocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public LocationsRepository(DirectoryServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _dbContext.Locations.AddAsync(location, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return location.Id.Value;
    }

    public async Task<Result<Location, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(d => d.Id.Value == id, cancellationToken);

        if (location is null)
        {
            return Error.Failure("location.not.found", "Location not found");
        }

        return Result.Success<Location, Error>(location);
    }

    public async Task<UnitResult<Error>> IsLocationActiveAsync(LocationId[] locationIds, CancellationToken cancellationToken)
    {
        foreach (var locationId in locationIds)
        {
            bool exists = await _dbContext.Locations
                .AnyAsync(
                    l => l.Id == locationId
                         && l.IsActive, cancellationToken);

            if (!exists)
            {
                return Error.Failure($"location{locationId}.not_active", locationId.ToString());
            }
        }

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> BulkDeleteInactiveLocationsAsync(
        List<DepartmentId> departmentIds,
        CancellationToken cancellationToken)
    {
        var connection = _dbContext.Database.GetDbConnection();
        var ids = departmentIds.Select(d => d.Value).ToArray();

        string sql = $@"
        WITH deleted_dl AS (
            DELETE FROM {Constants.DEPARTMENT_LOCATIONS_TABLE_ROUTE}
            WHERE department_id = ANY(@Ids)
            RETURNING 1
        ),
        deleted_locations AS (
            DELETE FROM {Constants.LOCATION_TABLE_ROUTE} l
            WHERE l.is_active = FALSE
              AND NOT EXISTS (
                  SELECT 1
                  FROM {Constants.DEPARTMENT_LOCATIONS_TABLE_ROUTE} dl
                  WHERE dl.location_id = l.id
              )
            RETURNING 1
        )
        SELECT 1;
    ";

        await connection.ExecuteAsync(sql, new { Ids = ids });

        return UnitResult.Success<Error>();
    }


    public async Task<UnitResult<Error>> DeleteInactiveAsync(CancellationToken cancellationToken)
    {
        string sql = $@"
        DELETE FROM {Constants.LOCATION_TABLE_ROUTE}
        WHERE is_active = false
          AND NOT EXISTS (
                SELECT 1
                FROM {Constants.DEPARTMENT_LOCATIONS_TABLE_ROUTE} dl
                WHERE dl.location_id = {Constants.LOCATION_TABLE_ROUTE}.id
          );
    ";

        await _dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);

        return UnitResult.Success<Error>();
    }



    public async Task<bool> IsAddressExistsAsync(Address address, CancellationToken cancellationToken)
    {
        bool location = await _dbContext.Locations
            .AnyAsync(l => l.Address == address, cancellationToken);

        return location;
    }

    public async Task<bool> IsNameUniqueAsync(LocationName name, CancellationToken cancellationToken)
    {
        bool location = await _dbContext.Locations
            .AnyAsync(l => l.Name == name, cancellationToken);

        return location;
    }
}