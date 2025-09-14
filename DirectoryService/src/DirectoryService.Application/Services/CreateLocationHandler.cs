using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.IRepositories;
using DirectoryService.Contracts.Locations;
using Microsoft.Extensions.Logging;
using Guid = System.Guid;

namespace DirectoryService.Application.Services;

public class CreateLocationHandler()
{
    private readonly ILocationsRepository _locationsRepository = null!;

    private readonly ILogger<CreateLocationHandler> _logger = null!;

    public CreateLocationHandler(ILocationsRepository locationsRepository, ILogger<CreateLocationHandler> logger)
        : this()
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateLocationDto locationDto, CancellationToken cancellationToken)
    {
        // validate
        // TODO: inject validator
        if(locationDto == null)
            throw new ArgumentNullException(nameof(locationDto));

        // create entity
        var name = LocationName.Create(locationDto.Name.Value);

        var address = Address.Create(
            postalCode: locationDto.Address.PostalCode,
            region: locationDto.Address.Region,
            city: locationDto.Address.City,
            street: locationDto.Address.Street,
            house: locationDto.Address.House,
            apartment: locationDto.Address.Apartment);

        var timeZone = Timezone.Create(
            locationDto.Timezone.Value);

        var location = Location.Create(
            name.Value,
            address.Value,
            timeZone.Value,
            DateTime.UtcNow,
            locationDto.IsActive
            //departmentLocations
            );

        // save to db
        var locationId = await _locationsRepository.AddAsync(location.Value, cancellationToken);

        // log result
        _logger.LogInformation($"Location created successfully with id {location.Value.Id}", location.Value.Id);

        // return result
        return Result.Success(locationId);
    }

/*
    public async Task Update(Guid id, UpdateLocationDto locationDto, CancellationToken cancellationToken)
    {
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
    }

    public async Task Get(Guid id, CancellationToken cancellationToken)
    {
    }
    */
}