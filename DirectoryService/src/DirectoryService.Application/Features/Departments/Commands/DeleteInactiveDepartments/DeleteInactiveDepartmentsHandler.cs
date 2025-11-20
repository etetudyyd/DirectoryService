using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Database.ITransactions;
using DirectoryService.Contracts.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Features.Departments.Commands.DeleteInactiveDepartments;

public class DeleteInactiveDepartmentsHandler : ICommandHandler<DeleteInactiveDepartmentsCommand>
{
    private readonly ILogger<DeleteInactiveDepartmentsHandler> _logger;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IPositionsRepository _positionsRepository;
    private readonly ITransactionManager _transactionManager;

    public DeleteInactiveDepartmentsHandler(
        ILogger<DeleteInactiveDepartmentsHandler> logger,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager, ILocationsRepository locationsRepository, IPositionsRepository positionsRepository)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _locationsRepository = locationsRepository;
        _positionsRepository = positionsRepository;
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
                .Select(d => d.Id).ToList(),
                cancellationToken);
        if (childrenDepartmentsResult.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return childrenDepartmentsResult.Error.ToErrors();
        }

        if (childrenDepartmentsResult.Value.Count != 0)
        {
            // получение родителей неактивных департаментов
            var parentDepartmentsResult = await _departmentsRepository
                .GetParentDepartmentsAsync(
                    inactiveDepartments
                        .Select(d => d.ParentId!).ToList(), cancellationToken);
            if (parentDepartmentsResult.IsFailure)
            {
                transactionScope.Rollback(cancellationToken);
                return childrenDepartmentsResult.Error.ToErrors();
            }

            var moves = new List<UpdatePath>();

            foreach (var child in childrenDepartmentsResult.Value)
            {
                var oldParent = departmentsResult.Value
                    .FirstOrDefault(d => d.Id == child.ParentId);

                var newParent = parentDepartmentsResult.Value
                    .FirstOrDefault(d => d.Id == oldParent?.ParentId);

                var oldPath = child.Path;
                int depthDelta = child.SetParent(newParent).Value;

                moves.Add(new UpdatePath(oldPath.Value, child.Path.Value, depthDelta));
            }

            var updateResult = await _departmentsRepository.BulkUpdateDescendantsPathAsync(
                moves, cancellationToken);

            if (updateResult.IsFailure)
            {
                _logger.LogInformation("Failed to update descendant departments.");
                transactionScope.Rollback(cancellationToken);
                return updateResult.Error.ToErrors();
            }
        }

        var saveChanges = await _departmentsRepository.SaveChangesAsync(cancellationToken);
        if (saveChanges.IsFailure)
        {
            _logger.LogInformation("Failed to save changes.");
            transactionScope.Rollback(cancellationToken);
            return saveChanges.Error.ToErrors();
        }

        await _locationsRepository.BulkDeleteInactiveLocationsAsync(
            inactiveDepartments.Select(d => d.Id).ToList(), cancellationToken);

        await _positionsRepository.BulkDeleteInactivePositionsAsync(
            inactiveDepartments.Select(d => d.Id).ToList(), cancellationToken);

        await _departmentsRepository.DeleteDepartmentsAsync(
            inactiveDepartments.Select(d => d.Id).ToList(), cancellationToken);

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

public record UpdatePath(string OldPath, string NewPath, int DepthDelta);