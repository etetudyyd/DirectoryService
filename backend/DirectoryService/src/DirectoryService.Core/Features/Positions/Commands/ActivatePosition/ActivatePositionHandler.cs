using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Commands.ActivatePosition;

public class ActivatePositionHandler : ICommandHandler<Guid, ActivatePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly ILogger<ActivatePositionHandler> _logger;

    private readonly HybridCache _cache;

    public ActivatePositionHandler(
        IPositionsRepository positionsRepository,
        ITransactionManager transactionManager,
        ILogger<ActivatePositionHandler> logger,
        HybridCache cache)
    {
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(ActivatePositionCommand command, CancellationToken cancellationToken)
    {
        var positionResult = await _positionsRepository
            .GetByIdWithLockAsync(command.Id, cancellationToken);
        if (positionResult.IsFailure)
        {
            return positionResult.Error.ToErrors();
        }

        var position = positionResult.Value;

        position.Activate();

        var saveChangesResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
            return saveChangesResult.Error.ToErrors();

        await _cache.RemoveByTagAsync(Constants.POSITION_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Position was activated with id{position.Id}");

        return position.Id.Value;
    }
}