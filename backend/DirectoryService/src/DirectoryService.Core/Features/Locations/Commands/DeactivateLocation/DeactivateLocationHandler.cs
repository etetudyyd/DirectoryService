using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Commands.DeactivateLocation;

public class DeactivateLocationHandler : ICommandHandler<Guid, DeactivateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly ILogger<DeactivateLocationHandler> _logger;

    private readonly IValidator<DeactivateLocationCommand> _validator;

    private readonly HybridCache _cache;

    public DeactivateLocationHandler(
        ILogger<DeactivateLocationHandler> logger,
        IValidator<DeactivateLocationCommand> validator,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        HybridCache cache)
    {
        _logger = logger;
        _validator = validator;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(
        DeactivateLocationCommand command,
        CancellationToken cancellationToken)
    {
      var validationResult = await _validator.ValidateAsync(command, cancellationToken);
      if (!validationResult.IsValid)
      {
          _logger.LogError("Invalid DepartmentDto");
          return validationResult.ToErrors();
      }

      var (_, isFailure, transaction, error) = await _transactionManager
          .BeginTransactionAsync(cancellationToken);
      if (isFailure)
          return error.ToErrors();

      var locationResult = await _locationsRepository
          .GetByIdWithLockAsync(command.Id, cancellationToken);
      if (locationResult.IsFailure)
      {
          transaction.Rollback(cancellationToken);
          return locationResult.Error.ToErrors();
      }

      var location = locationResult.Value;

      location.Deactivate();

      var saveChangesResult = await _transactionManager
          .SaveChangesAsync(cancellationToken);
      if (saveChangesResult.IsFailure)
          return saveChangesResult.Error.ToErrors();

      var commitResult = transaction.Commit(cancellationToken);
      if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();

      await _cache.RemoveByTagAsync(Constants.LOCATION_CACHE_PREFIX, cancellationToken);

      _logger.LogInformation($"Location was deactivated with id{location.Id}");

      return location.Id.Value;
    }
}