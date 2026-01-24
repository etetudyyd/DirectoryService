using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.Location;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Commands.UpdateLocation;

public class UpdateLocationHandler : ICommandHandler<Location, UpdateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly UpdateLocationCommandValidator _validator;

    private readonly ILogger<UpdateLocationHandler> _logger;

    private readonly HybridCache _cache;

    public UpdateLocationHandler(
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        UpdateLocationCommandValidator validator,
        ILogger<UpdateLocationHandler> logger,
        HybridCache cache)
    {
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
        _cache = cache;
    }


    public async Task<Result<Location, Errors>> Handle(
        UpdateLocationCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Invalid LocationDto!");
            return validationResult.ToErrors();
        }

        var (_, isFailure, transaction, error) = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (isFailure)
            return error.ToErrors();

        var locationResult = await _locationsRepository
            .GetByIdWithLockAsync(command.Id, cancellationToken);
        if (locationResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            _logger.LogError("Location is not active!");
            return locationResult.Error.ToErrors();
        }

        var location = locationResult.Value;

        // starting preparing data to update
        var updatedName = LocationName.Create(
            command.LocationRequest.Name);
        if (updatedName.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            _logger.LogError("Location name wasn't updated!");
            return updatedName.Error.ToErrors();
        }

        var updatedAddress = Address.Create(
            command.LocationRequest.Address.PostalCode,
            command.LocationRequest.Address.Region,
            command.LocationRequest.Address.City,
            command.LocationRequest.Address.Street,
            command.LocationRequest.Address.House,
            command.LocationRequest.Address.Apartment);

        if(updatedAddress.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            _logger.LogError("Location address wasn't updated!");
            return updatedName.Error.ToErrors();
        }

        var updatedTimezone = Timezone.Create(
            command.LocationRequest.Timezone);

        if(updatedTimezone.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            _logger.LogError("Location timezone wasn't updated!");
            return updatedName.Error.ToErrors();
        }

        var updateLocationResult = location.Update(updatedName.Value, updatedAddress.Value, updatedTimezone.Value);
        if (updateLocationResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            _logger.LogError("Location wasn't updated!");
            return updateLocationResult.Error.ToErrors();
        }

        var saveChangesResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
            return saveChangesResult.Error.ToErrors();

        var commitResult = transaction.Commit(cancellationToken);
        if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();

        await _cache.RemoveByTagAsync(Constants.LOCATION_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Location was deactivated with id{location.Id}");

        return location;
    }
}