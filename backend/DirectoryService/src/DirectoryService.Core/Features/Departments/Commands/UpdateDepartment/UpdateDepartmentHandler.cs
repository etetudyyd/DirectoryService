using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using DirectoryService.ValueObjects.Department;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentHandler : ICommandHandler<Guid, UpdateDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly UpdateDepartmentCommandValidator _validator;

    private readonly ILogger<UpdateDepartmentHandler> _logger;

    private readonly HybridCache _cache;

    public UpdateDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        UpdateDepartmentCommandValidator validator,
        ILogger<UpdateDepartmentHandler> logger,
        HybridCache cache)
    {
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
         var validationResult = await _validator.ValidateAsync(command, cancellationToken);
         if (!validationResult.IsValid)
         {
             _logger.LogError("Invalid DepartmentDto!");
             return validationResult.ToErrors();
         }

         var updatedDepartment = command.Request;

         (_, bool isFailure, ITransactionScope? transaction, Error? error) = await _transactionManager
             .BeginTransactionAsync(cancellationToken);
         if (isFailure)
             return error.ToErrors();

         var departmentResult = await _departmentsRepository
             .GetBy(x => x.Id == new DepartmentId(command.DepartmentId), cancellationToken);
         if (departmentResult.IsFailure)
         {
             _logger.LogError("Department is not active!");
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

             // starting preparing data to update
         var updatedName = DepartmentName.Create(
             updatedDepartment.Name);
         if (updatedName.IsFailure)
         {
             _logger.LogError("Department name wasn't updated! Update name failure");
             transaction.Rollback(cancellationToken);
             return updatedName.Error.ToErrors();
         }

         var updatedIdentifier = Identifier.Create(
             updatedDepartment.Identifier);

         if (updatedIdentifier.IsFailure)
         {
             _logger.LogError("Department name wasn't updated! Update identifier failure");
             transaction.Rollback(cancellationToken);
             return updatedIdentifier.Error.ToErrors();
         }

         var oldIdentifier = department.Identifier;
         string oldPath = department.Path.Value;

         var updateDepartmentResult = department.Update(updatedName.Value, oldIdentifier, updatedIdentifier.Value);
         if (updateDepartmentResult.IsFailure)
         {
             _logger.LogError("Department wasn't updated!");
             transaction.Rollback(cancellationToken);
             return updateDepartmentResult.Error.ToErrors();
         }

         var saveChangesResult = await _transactionManager
             .SaveChangesAsync(cancellationToken);
         if (saveChangesResult.IsFailure)
         {
             transaction.Rollback(cancellationToken);
             return saveChangesResult.Error.ToErrors();
         }

         var updateChildDepartmentsResult = await _departmentsRepository
             .UpdateChildDepartmentsPath(oldPath, department.Path.Value, department.Id.Value, cancellationToken);
         if (updateChildDepartmentsResult.IsFailure)
         {
             _logger.LogError("Department's children paths weren't updated!");
             transaction.Rollback(cancellationToken);
             return updateChildDepartmentsResult.Error.ToErrors();
         }

         var commitResult = transaction.Commit(cancellationToken);
         if (commitResult.IsFailure)
             return commitResult.Error.ToErrors();

         await _cache.RemoveByTagAsync(Constants.DEPARTMENT_CACHE_PREFIX, cancellationToken);

         _logger.LogInformation($"Department was deactivated with id{department.Id}");

         return department.Id.Value;
    }
}