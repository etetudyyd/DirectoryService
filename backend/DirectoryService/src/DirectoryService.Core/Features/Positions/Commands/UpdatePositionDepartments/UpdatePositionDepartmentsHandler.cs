using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Database.ITransactions;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Position;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Commands.UpdatePositionDepartments;

public class UpdatePositionDepartmentsHandler : ICommandHandler<Guid, UpdatePositionDepartmentsCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly IPositionsRepository _positionsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly UpdatePositionDepartmentsCommandValidator _validator;

    private readonly ILogger<UpdatePositionDepartmentsHandler> _logger;

    private readonly HybridCache _cache;

    public UpdatePositionDepartmentsHandler(
        IDepartmentsRepository departmentsRepository,
        IPositionsRepository positionsRepository,
        ITransactionManager transactionManager,
        UpdatePositionDepartmentsCommandValidator validator,
        ILogger<UpdatePositionDepartmentsHandler> logger, HybridCache cache)
    {
        _departmentsRepository = departmentsRepository;
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Guid, Errors>> Handle(
        UpdatePositionDepartmentsCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Invalid PositionDto");
            return validationResult.ToErrors();
        }

        var positionId = new PositionId(command.PositionId);

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();

        using var transactionScope = transactionScopeResult.Value;

        var position = await _positionsRepository
            .GetByIdAsync(positionId.Value, cancellationToken);

        if (position.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return position.Error.ToErrors();
        }

        var departmentIds = position.Value.DepartmentPositions
            .Select(x => x.DepartmentId)
            .ToArray();

        var departmentPositionsResult = await _departmentsRepository
            .IsDepartmentsActiveAsync(departmentIds, cancellationToken);

        var updatedDepartments = command.PositionDepartmentsRequest.DepartmentsIds
            .Select(dp => new DepartmentId(dp))
            .ToArray();

        var positionDepartmentsUpdateResult = await _departmentsRepository
            .IsDepartmentsActiveAsync(updatedDepartments, cancellationToken);

        if (departmentPositionsResult.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return departmentPositionsResult.Error.ToErrors();
        }

        if (positionDepartmentsUpdateResult.IsFailure)
        {
            transactionScope.Rollback(cancellationToken);
            return positionDepartmentsUpdateResult.Error.ToErrors();
        }

        List<DepartmentPosition> departmentPositions = [];

        foreach (var positionDepartmentsRequest in command.PositionDepartmentsRequest.DepartmentsIds)
        {
            var departmentPosition = DepartmentPosition.Create(
                positionId,
                new DepartmentId(positionDepartmentsRequest));

            if (departmentPosition.IsFailure)
            {
                transactionScope.Rollback(cancellationToken);
                return departmentPosition.Error.ToErrors();
            }

            departmentPositions.Add(departmentPosition.Value);
        }

        position.Value.UpdateDepartments(departmentPositions);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        var commitedResult = transactionScope.Commit(cancellationToken);
        if (commitedResult.IsFailure)
        {
            return commitedResult.Error.ToErrors();
        }

        await _cache.RemoveByTagAsync(Constants.POSITION_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Position was updated with id{positionId.Value}");

        return positionId.Value;
    }
}