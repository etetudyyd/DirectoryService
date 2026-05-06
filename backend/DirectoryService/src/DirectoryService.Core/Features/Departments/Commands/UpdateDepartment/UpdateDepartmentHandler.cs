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

         var departmentResult = await _departmentsRepository
             .GetBy(x => x.Id == new DepartmentId(command.DepartmentId), cancellationToken);
         if (departmentResult.IsFailure)
         {
             _logger.LogError("Department is not active!");
             return departmentResult.Error.ToErrors();
         }

         var department = departmentResult.Value;

             // starting preparing data to update
         var updatedName = DepartmentName.Create(
             updatedDepartment.Name);
         if (updatedName.IsFailure)
         {
             _logger.LogError("Department name wasn't updated! Update name failure");
             return updatedName.Error.ToErrors();
         }

         var updatedIdentifier = Identifier.Create(
             updatedDepartment.Identifier);

         if (updatedIdentifier.IsFailure)
         {
             _logger.LogError("Department name wasn't updated! Update identifier failure");
             return updatedIdentifier.Error.ToErrors();
         }


         var updateDepartmentResult = department.Update(updatedName.Value, updatedIdentifier.Value);
         if (updateDepartmentResult.IsFailure)
         {
             _logger.LogError("Department wasn't updated!");
             return updateDepartmentResult.Error.ToErrors();
         }

         var saveChangesResult = await _transactionManager
             .SaveChangesAsync(cancellationToken);
         if (saveChangesResult.IsFailure)
             return saveChangesResult.Error.ToErrors();

         await _cache.RemoveByTagAsync(Constants.DEPARTMENT_CACHE_PREFIX, cancellationToken);

         _logger.LogInformation($"Department was deactivated with id{department.Id}");

         return department.Id.Value;
    }
}