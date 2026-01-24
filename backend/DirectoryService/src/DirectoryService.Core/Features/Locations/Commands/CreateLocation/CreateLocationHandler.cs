using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.ConnectionEntities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Location;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;
using Guid = System.Guid;

namespace DirectoryService.Features.Locations.Commands.CreateLocation;

public class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;

    private readonly ILogger<CreateLocationHandler> _logger;

    private readonly IValidator<CreateLocationCommand> _validator;

    private HybridCache _cache;

    public CreateLocationHandler(
        ILocationsRepository locationsRepository,
        ILogger<CreateLocationHandler> logger,
        IValidator<CreateLocationCommand> validator, 
        HybridCache cache)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
        _validator = validator;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(
        CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        // validate
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Invalid LocationDto");
            return validationResult.ToErrors();
        }

        // create entity
        var name = LocationName.Create(command.LocationRequest.Name);
        if (name.IsFailure)
        {
            _logger.LogError("Invalid LocationDto.Name");
            return name.Error.ToErrors();
        }

        var address = Address.Create(
            postalCode: command.LocationRequest.Address.PostalCode,
            region: command.LocationRequest.Address.Region,
            city: command.LocationRequest.Address.City,
            street: command.LocationRequest.Address.Street,
            house: command.LocationRequest.Address.House,
            apartment: command.LocationRequest.Address.Apartment);

        if (address.IsFailure)
        {
            _logger.LogError("Invalid LocationDto.Address");
            return address.Error.ToErrors();
        }

        bool isNameExists = await _locationsRepository.IsNameUniqueAsync(name.Value, cancellationToken);

        if (isNameExists)
        {
            _logger.LogError("Name already exists");
            return GeneralErrors.General.ValueAlreadyExists("Name").ToErrors();
        }

        bool isAddressExists = await _locationsRepository.IsAddressExistsAsync(address.Value, cancellationToken);

        if (isAddressExists)
        {
            _logger.LogError("Address already exists");
            return GeneralErrors.General.ValueAlreadyExists("Address").ToErrors();
        }

        var timeZone = Timezone.Create(
            command.LocationRequest.Timezone);

        if (timeZone.IsFailure)
        {
            _logger.LogError("Invalid LocationDto.TimeZone");
            return timeZone.Error.ToErrors();
        }

        var locationId = new LocationId(Guid.NewGuid());

        var departmentLocations = command.LocationRequest.DepartmentsIds
            .Select(departmentId => new DepartmentLocation(
                new DepartmentLocationId(Guid.NewGuid()),
                new DepartmentId(departmentId),
                locationId))
            .ToList();

        var location = Location.Create(
            locationId,
            name.Value,
            address.Value,
            timeZone.Value,
            departmentLocations);

        if (location.IsFailure)
        {
            _logger.LogError("Invalid LocationDto.Location");
            return location.Error.ToErrors();
        }

        // save to db
        await _locationsRepository.AddAsync(location.Value, cancellationToken);

        await _cache.RemoveByTagAsync(Constants.LOCATION_CACHE_PREFIX, cancellationToken);

        // log result
        _logger.LogInformation($"Location created successfully with id {locationId}", locationId);

        // return result
        return locationId.Value;
    }
}