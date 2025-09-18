using DevQuestions.Domain.Entities;
using DirectoryService.Application.IRepositories;

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

    public Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task UpdateAsync(Guid id, Location location, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<Location> GetByIdAsync(Guid id, CancellationToken cancellationToken) => throw new NotImplementedException();
}