using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using DirectoryService.Features.Locations.Commands.DeactivateLocation;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Commands.ActivateLocation;

public class ActivateLocationHandler : ICommandHandler<Guid, ActivateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly ILogger<ActivateLocationHandler> _logger;

    private readonly HybridCache _cache;

    public ActivateLocationHandler(
        ILogger<ActivateLocationHandler> logger,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        HybridCache cache)
    {
        _logger = logger;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(ActivateLocationCommand command, CancellationToken cancellationToken)
    {

        var locationResult = await _locationsRepository
            .GetByIdWithLockAsync(command.Id, cancellationToken);
        if (locationResult.IsFailure)
        {
            _logger.LogError("Location is not active!");
            return locationResult.Error.ToErrors();
        }

        var location = locationResult.Value;

        location.Activate();

        var saveChangesResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
            return saveChangesResult.Error.ToErrors();

        await _cache.RemoveByTagAsync(Constants.LOCATION_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Location was activated with id{location.Id}");

        return location.Id.Value;
    }
}