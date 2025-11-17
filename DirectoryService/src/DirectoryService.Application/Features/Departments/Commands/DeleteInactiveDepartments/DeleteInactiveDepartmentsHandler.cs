using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Database.ITransactions;
using DirectoryService.Application.Extentions;
using DirectoryService.Contracts.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Features.Departments.Commands.DeleteInactiveDepartments;

public class DeleteInactiveDepartmentsHandler : ICommandHandler<DeleteInactiveDepartmentsCommand>
{
    private readonly ILogger<DeleteInactiveDepartmentsHandler> _logger;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;

    public DeleteInactiveDepartmentsHandler(
        ILogger<DeleteInactiveDepartmentsHandler> logger,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<UnitResult<Errors>> Handle(
        DeleteInactiveDepartmentsCommand command,
        CancellationToken cancellationToken)
    {
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();

        using var transactionScope = transactionScopeResult.Value;

        TimeOptions timeOptions = TimeOptions.FromMonthsAgo(1);
        // получение неактивных департаментов
        var departmentsResult = await _departmentsRepository
            .GetAllInactiveDepartmentsAsync(timeOptions, cancellationToken);

        if (departmentsResult.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            _logger.LogInformation("Departments for deleting was not found!");
            return departmentsResult.Error.ToErrors();
        }

        var inactiveDepartments = departmentsResult.Value;

        // получение первых детей неактивных департаментов
        var childrenDepartmentsResult = await _departmentsRepository
            .GetChildrenDepartmentsAsync(
                inactiveDepartments
                .Select(d => d.Id.Value).ToArray(),
                cancellationToken);
        if (childrenDepartmentsResult.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return childrenDepartmentsResult.Error.ToErrors();
        }

        // получение родителей неактивных департаментов
        var parentDepartmentsResult = await _departmentsRepository
            .GetParentDepartmentsAsync(
                inactiveDepartments
                .Select(d => d.ParentId!.Value).ToArray(), cancellationToken);
        if(parentDepartmentsResult.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return childrenDepartmentsResult.Error.ToErrors();
        }

        foreach (var children in childrenDepartmentsResult.Value)
        {
            var oldParent = departmentsResult.Value
                .FirstOrDefault(d => d.Id == children.ParentId);
            var newParent = parentDepartmentsResult.Value
                .FirstOrDefault(d => d.Id == oldParent?.ParentId);
            var oldPath = children.Path;
            int depthDelta = children.SetParent(newParent).Value;

            var updateDescendantDepartmentsResult = await _departmentsRepository
                .BulkUpdateDescendantsPath(
                oldPath,
                children.Path,
                depthDelta,
                cancellationToken);
            if (updateDescendantDepartmentsResult.IsFailure)
            {
                _logger.LogInformation("Failed to update descendant departments.");
                transactionScope.Rollback(cancellationToken);
                return updateDescendantDepartmentsResult.Error.ToErrors();
            }
        }

        var saveChanges = await _departmentsRepository.SaveChangesAsync(cancellationToken);
        if (saveChanges.IsFailure)
        {
            _logger.LogInformation("Failed to save changes.");
            transactionScope.Rollback(cancellationToken);
            return saveChanges.Error.ToErrors();
        }

        await _departmentsRepository.BulkDeleteAsync(
            inactiveDepartments.Select(d => d.Id.Value).ToArray(), cancellationToken);

        var commitResult = transactionScope.Commit(cancellationToken);
        if (commitResult.IsFailure)
        {
            _logger.LogInformation("Failed to commit transaction.");
            transactionScope.Rollback(cancellationToken);
            return commitResult.Error.ToErrors();
        }

        return UnitResult.Success<Errors>();
    }
}