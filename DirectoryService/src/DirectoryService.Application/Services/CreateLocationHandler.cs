using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Extentions;
using DirectoryService.Application.IRepositories;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Guid = System.Guid;

namespace DirectoryService.Application.Services;

public class CreateLocationHandler
{
    private readonly ILocationsRepository _locationsRepository;

    private readonly ILogger<CreateLocationHandler> _logger;

    private readonly IValidator<CreateLocationDto> _validator;

    public CreateLocationHandler(
        ILocationsRepository locationsRepository,
        ILogger<CreateLocationHandler> logger,
        IValidator<CreateLocationDto> validator)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(CreateLocationDto locationDto, CancellationToken cancellationToken)
    {
        // validate
        var validationResult = await _validator.ValidateAsync(locationDto, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        // create entity
        var name = LocationName.Create(locationDto.Name.Value);
        if (name.IsFailure)
            return name.Error.ToErrors();

        var address = Address.Create(
            postalCode: locationDto.Address.PostalCode,
            region: locationDto.Address.Region,
            city: locationDto.Address.City,
            street: locationDto.Address.Street,
            house: locationDto.Address.House,
            apartment: locationDto.Address.Apartment);

        if (address.IsFailure)
            return address.Error.ToErrors();

        var timeZone = Timezone.Create(
            locationDto.Timezone.Value);

        if (timeZone.IsFailure)
            return timeZone.Error.ToErrors();

        var location = Location.Create(
            name.Value,
            address.Value,
            timeZone.Value,
            locationDto.DepartmentLocations);

        if (location.IsFailure)
            return location.Error.ToErrors();

        // save to db
        var locationId = await _locationsRepository.AddAsync(location.Value, cancellationToken);

        // log result
        _logger.LogInformation($"Location created successfully with id {location.Value.Id}", location.Value.Id);

        // return result
        return locationId;
    }
}