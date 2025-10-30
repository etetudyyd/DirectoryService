using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Database.ITransactions;
using DirectoryService.Application.Extentions;
using DirectoryService.Application.Features.Departments.Commands.CreateDepartment;
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

        // удаление департамента
        var result = await _departmentsRepository.Delete(departmentResult.Value, cancellationToken);
        if (result.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return result.Error.ToErrors();
        }

        // TODO деактивирование локаций связаных только с этим подразделением
        //
        // TODO обновление пути у дочерних подразделений
        var commitResult = transaction.Commit(cancellationToken);
        if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();

        _logger.LogInformation($"Department deleted with id{result.Value}");

        return result.Value;

    }
}