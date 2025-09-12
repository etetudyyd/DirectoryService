using DevQuestions.Domain.Entities;
using DirectoryService.Contracts;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken);

    Task UpdateAsync(Guid id, Location location, CancellationToken cancellationToken);

    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Location> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}