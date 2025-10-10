using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Extentions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Guid = System.Guid;

namespace DirectoryService.Application.CQRS.Locations.Commands.CreateLocation;

public class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;

    private readonly ILogger<CreateLocationHandler> _logger;

    private readonly IValidator<CreateLocationCommand> _validator;

    public CreateLocationHandler(
        ILocationsRepository locationsRepository,
        ILogger<CreateLocationHandler> logger,
        IValidator<CreateLocationCommand> validator)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
        _validator = validator;
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
        var name = LocationName.Create(command.LocationRequest.Name.Value);
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

        bool isAddressExists = await _locationsRepository.IsAddressExistsAsync(address.Value, cancellationToken);

        if (isAddressExists)
        {
            _logger.LogError("Address already exists");
            return GeneralErrors.General.ValueAlreadyExists("Address").ToErrors();
        }

        if (address.IsFailure)
        {
            _logger.LogError("Invalid LocationDto.Address");
            return address.Error.ToErrors();
        }

        var timeZone = Timezone.Create(
            command.LocationRequest.Timezone.Value);

        if (timeZone.IsFailure)
        {
            _logger.LogError("Invalid LocationDto.TimeZone");
            return timeZone.Error.ToErrors();
        }

        var location = Location.Create(
            name.Value,
            address.Value,
            timeZone.Value,
            command.LocationRequest.DepartmentLocations);

        if (location.IsFailure)
        {
            _logger.LogError("Invalid LocationDto.Location");
            return location.Error.ToErrors();
        }

        // save to db
        var locationId = await _locationsRepository.AddAsync(location.Value, cancellationToken);

        // log result
        _logger.LogInformation($"Location created successfully with id {locationId}", locationId);

        // return result
        return locationId;
    }
}