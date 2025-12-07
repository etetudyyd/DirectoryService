using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Commands.DeactivateDepartment;

public class DeactivateDepartmentHandler : ICommandHandler<Guid, DeactivateDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly ILogger<DeactivateDepartmentHandler> _logger;

    private readonly IValidator<DeactivateDepartmentCommand> _validator;

    private readonly HybridCache _cache;

    public DeactivateDepartmentHandler(
        ILogger<DeactivateDepartmentHandler> logger,
        IValidator<DeactivateDepartmentCommand> validator,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager, HybridCache cache)
    {
        _logger = logger;
        _validator = validator;
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(
        DeactivateDepartmentCommand command,
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

        var departmentResult = await _departmentsRepository
            .GetByIdAsync(command.Id, cancellationToken);
        if (departmentResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return departmentResult.Error.ToErrors();
        }

        var department = departmentResult.Value;

        var lockDescendantsResult = await _departmentsRepository
            .LockDescendantsAsync(department, cancellationToken);
        if (lockDescendantsResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return lockDescendantsResult.Error.ToErrors();
        }

        department.Deactivate();

        var saveChangesResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
            return saveChangesResult.Error.ToErrors();

        var updateChildDepartmentsPathResult = await _departmentsRepository
            .UpdateChildDepartmentsPath(department, cancellationToken);
        if (updateChildDepartmentsPathResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return updateChildDepartmentsPathResult.Error.ToErrors();
        }

        var deactivateConnectedLocationsResult = await _departmentsRepository
            .DeactivateConnectedLocations(department.Id, cancellationToken);
        if (deactivateConnectedLocationsResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return deactivateConnectedLocationsResult.Error.ToErrors();
        }

        var deactivateConnectedPositions = await _departmentsRepository
            .DeactivateConnectedPositions(department.Id, cancellationToken);
        if (deactivateConnectedPositions.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return deactivateConnectedPositions.Error.ToErrors();
        }

        var commitResult = transaction.Commit(cancellationToken);
        if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();

        await _cache.RemoveByTagAsync(Constants.DEPARTMENT_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Department was deactivated with id{department.Id}");

        return department.Id.Value;

    }
}