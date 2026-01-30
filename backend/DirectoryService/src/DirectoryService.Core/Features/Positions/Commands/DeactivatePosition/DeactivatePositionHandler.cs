using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Commands.DeactivatePosition;

public class DeactivatePositionHandler : ICommandHandler<Guid, DeactivatePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly ILogger<DeactivatePositionHandler> _logger;

    private readonly IValidator<DeactivatePositionCommand> _validator;

    private readonly HybridCache _cache;

    public DeactivatePositionHandler(
        IPositionsRepository positionsRepository,
        ITransactionManager transactionManager,
        ILogger<DeactivatePositionHandler> logger,
        IValidator<DeactivatePositionCommand> validator,
        HybridCache cache)
    {
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(
        DeactivatePositionCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Invalid PositionDto");
            return validationResult.ToErrors();
        }

        var (_, isFailure, transaction, error) = await _transactionManager
            .BeginTransactionAsync(cancellationToken);
        if (isFailure)
            return error.ToErrors();

        var positionResult = await _positionsRepository
            .GetByIdWithLockAsync(command.Id, cancellationToken);
        if (positionResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return positionResult.Error.ToErrors();
        }

        var position = positionResult.Value;

        position.Deactivate();

        var saveChangesResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
            return saveChangesResult.Error.ToErrors();

        var commitResult = transaction.Commit(cancellationToken);
        if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();

        await _cache.RemoveByTagAsync(Constants.POSITION_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Position was deactivated with id{position.Id}");

        return position.Id.Value;
    }
}