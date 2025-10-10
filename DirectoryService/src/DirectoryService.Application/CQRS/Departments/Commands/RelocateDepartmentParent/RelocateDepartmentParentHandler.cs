using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Database.ITransactions;
using DirectoryService.Application.Extentions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.CQRS.Departments.Commands.RelocateDepartmentParent;

public class RelocateDepartmentParentHandler : ICommandHandler<Guid, RelocateDepartmentParentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly ILogger<RelocateDepartmentParentHandler> _logger;

    private readonly IValidator<RelocateDepartmentParentCommand> _validator;

    private readonly ITransactionManager _transactionManager;

    public RelocateDepartmentParentHandler(
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        ILogger<RelocateDepartmentParentHandler> logger,
        IValidator<RelocateDepartmentParentCommand> validator)
    {
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(RelocateDepartmentParentCommand command, CancellationToken cancellationToken)
    {
        // validation
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Invalid DepartmentDto");
            return validationResult.ToErrors();
        }

        // open transaction
        var (_, isFailure, transaction, error) = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if (isFailure)
            return error.ToErrors();

        var getDepartmentResult =
            await _departmentsRepository.GetByIdWithLockAsync(command.DepartmentId, cancellationToken);
        if (getDepartmentResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return getDepartmentResult.Error.ToErrors();
        }

        var department = getDepartmentResult.Value;

        Department? parentDepartment = null;
        if (command.DepartmentRequest.ParentId != null)
        {
            var getParentResult = await _departmentsRepository
                .GetByIdWithLockAsync(command.DepartmentRequest.ParentId.Value, cancellationToken);
            if (getParentResult.IsFailure)
            {
                transaction.Rollback(cancellationToken);
                return getParentResult.Error.ToErrors();
            }

            parentDepartment = getParentResult.Value;
        }

        if (command.DepartmentRequest.ParentId != null)
        {
            var isDescendant = await _departmentsRepository.IsDescendantAsync(
                department.Path,
                command.DepartmentRequest.ParentId.Value,
                cancellationToken);
            if (isDescendant)
            {
                transaction.Rollback(cancellationToken);
                return Error.NotFound("department.not.found", "CannotAddChildAsAncestor").ToErrors();
            }
        }

        var lockDescendantsResult =
            await _departmentsRepository.LockDescendantsAsync(department, cancellationToken);
        if (lockDescendantsResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return lockDescendantsResult.Error.ToErrors();
        }

        // old path нужен для запроса на обновление путей в БД
        var oldPath = department.Path;

        // Меняем родителя, доменная модель посчитает новый path и depth для департмента
        department.SetParent(parentDepartment);

        // Обновляем также path и depth у всех потомков этого department с помощью ltree
        var updateResult = await _departmentsRepository.RelocateDepartmentAsync(department, oldPath, cancellationToken);
        if (updateResult.IsFailure)
        {
            transaction.Rollback(cancellationToken);
            return updateResult.Error.ToErrors();
        }

        var commitResult = transaction.Commit(cancellationToken);
        if (commitResult.IsFailure)
            return commitResult.Error.ToErrors();

        _logger.LogInformation(
            "Parent successfully updated Department id='{@Id}' new parent_id={@ParentId}",
            department.Id,
            department.ParentId);

        return department.Id.Value;
    }
   }