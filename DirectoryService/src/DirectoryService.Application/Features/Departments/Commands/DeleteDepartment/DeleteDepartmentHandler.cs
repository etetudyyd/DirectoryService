using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Database.ITransactions;
using DirectoryService.Application.Extentions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentHandler : ICommandHandler<Guid, DeleteDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly ILogger<DeleteDepartmentHandler> _logger;

    private readonly IValidator<DeleteDepartmentCommand> _validator;

    public DeleteDepartmentHandler(
        ILogger<DeleteDepartmentHandler> logger,
        IValidator<DeleteDepartmentCommand> validator,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager)
    {
        _logger = logger;
        _validator = validator;
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(
        DeleteDepartmentCommand command,
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

        department.Delete();

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

        _logger.LogInformation($"Department deleted with id{department.Id}");

        return department.Id.Value;

    }
}