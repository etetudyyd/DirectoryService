using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Entities.AdjacentEntities;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Locations;

public class LocationsService()
{
    private readonly ILocationsRepository _locationsRepository = null!;

    private readonly ILogger<LocationsService> _logger = null!;

    public LocationsService(ILocationsRepository locationsRepository, ILogger<LocationsService> logger)
        : this()
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<Guid> Create(CreateLocationDto locationDto, CancellationToken cancellationToken)
    {
        // validate
        if(locationDto == null)
            throw new ArgumentNullException(nameof(locationDto));

        // create entity
        var location = Location.Create(
            locationDto.Name,
            locationDto.Address,
            locationDto.Timezone,
            DateTime.UtcNow,
            new List<DepartmentLocation>(),
            new List<Department>());

        // save to db
        await _locationsRepository.AddAsync(location, cancellationToken);

        // log result
        _logger.LogInformation($"Location created successfully with id {locationId}", locationId);

        // return result
        return locationId;
    }

    public async Task Update(Guid id, UpdateLocationDto locationDto, CancellationToken cancellationToken)
    {
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
    }

    public async Task Get(Guid id, CancellationToken cancellationToken)
    {
    }
}