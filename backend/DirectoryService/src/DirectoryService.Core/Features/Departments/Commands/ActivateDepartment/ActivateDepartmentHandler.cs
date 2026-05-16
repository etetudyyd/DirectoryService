using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using DirectoryService.ValueObjects.Department;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Commands.ActivateDepartment;

public class ActivateDepartmentHandler : ICommandHandler<Guid, ActivateDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly ILogger<ActivateDepartmentHandler> _logger;

    private readonly HybridCache _cache;

    public ActivateDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        ILogger<ActivateDepartmentHandler> logger,
        HybridCache cache)
    {
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(ActivateDepartmentCommand command, CancellationToken cancellationToken)
    {
        (_, bool isFailure, ITransactionScope? transaction, Error? error) = await _transactionManager
            .BeginTransactionAsync(cancellationToken);
        if (isFailure)
            return error.ToErrors();

        var departmentResult = await _departmentsRepository
            .GetBy(x => x.Id == new DepartmentId(command.Id), cancellationToken);
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

        string oldPath = department.Path.Value;

        department.Activate();

        var saveChangesResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
            return saveChangesResult.Error.ToErrors();

        var updateChildDepartmentsPathResult = await _departmentsRepository
            .UpdateChildDepartmentsPath(
                oldPath,
                department.Path.Value,
                department.Id.Value,
                cancellationToken);

        if (updateChildDepartmentsPathResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return updateChildDepartmentsPathResult.Error.ToErrors();
        }

      /*
       *   var updateChildDepartmentsPathResult = await _departmentsRepository
            .UpdateChildDepartmentsPath(department, cancellationToken);
        if (updateChildDepartmentsPathResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return updateChildDepartmentsPathResult.Error.ToErrors();
        }
       */

        var commitResult = transaction.Commit(cancellationToken);
        if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();

        await _cache.RemoveByTagAsync(Constants.DEPARTMENT_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Department was activated with id{department.Id}");

        return department.Id.Value;
    }
}