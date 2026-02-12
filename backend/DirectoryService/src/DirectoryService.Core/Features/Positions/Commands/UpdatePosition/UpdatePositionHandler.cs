using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using DirectoryService.ValueObjects.Position;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Commands.UpdatePosition;

public class UpdatePositionHandler : ICommandHandler<Guid, UpdatePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly UpdatePositionCommandValidator _validator;

    private readonly ILogger<UpdatePositionHandler> _logger;

    private readonly HybridCache _cache;

    public UpdatePositionHandler(
        IPositionsRepository positionsRepository,
        ITransactionManager transactionManager,
        UpdatePositionCommandValidator validator,
        ILogger<UpdatePositionHandler> logger,
        HybridCache cache)
    {
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(
        UpdatePositionCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Invalid PositionDto!");
            return validationResult.ToErrors();
        }

        var positionResult = await _positionsRepository
            .GetBy(x => x.Id == new PositionId(command.Id), cancellationToken);
        if (positionResult.IsFailure)
        {
            _logger.LogError("Position is not active!");
            return positionResult.Error.ToErrors();
        }

        var position = positionResult.Value;

        // starting preparing data to update
        var updatedName = PositionName.Create(
            command.PositionRequest.Name);
        if (updatedName.IsFailure)
        {
            _logger.LogError("Location name wasn't updated!");
            return updatedName.Error.ToErrors();
        }

        var updatedDescription = PositionDescription.Create(
            command.PositionRequest.Description);

        if(updatedDescription.IsFailure)
        {
            _logger.LogError("Position description wasn't updated!");
            return updatedName.Error.ToErrors();
        }

        var updatePositionResult = position.Update(updatedName.Value, updatedDescription.Value);
        if (updatePositionResult.IsFailure)
        {
            _logger.LogError("Position wasn't updated!");
            return updatePositionResult.Error.ToErrors();
        }

        var saveChangesResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
            return saveChangesResult.Error.ToErrors();

        await _cache.RemoveByTagAsync(Constants.POSITION_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Location was deactivated with id{position.Id}");

        return position.Id.Value;
    }
}