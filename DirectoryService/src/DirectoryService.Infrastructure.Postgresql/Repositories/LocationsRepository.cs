using DevQuestions.Domain.Entities;
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

    public async Task<bool> IsAddressExistsAsync(Address address, CancellationToken cancellationToken)
    {
        bool location = await _dbContext.Locations
            .AnyAsync(l => l.Address == address, cancellationToken);

        return location;
    }
}