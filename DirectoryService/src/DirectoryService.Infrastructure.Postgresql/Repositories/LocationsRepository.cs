using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Infrastructure.Postgresql.Database;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgresql.Repositories;

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



    public async Task<bool> IsAddressExistsAsync(Address address, CancellationToken cancellationToken)
    {
        bool location = await _dbContext.Locations
            .AnyAsync(l => l.Address == address, cancellationToken);

        return location;
    }
}