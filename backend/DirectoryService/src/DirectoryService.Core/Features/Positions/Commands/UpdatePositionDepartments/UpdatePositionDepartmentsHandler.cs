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
    private readonly IPositionsRepository _positionsRepository;
    private readonly IDepartmentsRepository _departmentsRepository;

    private readonly ITransactionManager _transactionManager;

    private readonly UpdatePositionDepartmentsCommandValidator _validator;

    private readonly ILogger<UpdatePositionDepartmentsHandler> _logger;

    private readonly HybridCache _cache;

    public UpdatePositionDepartmentsHandler(
        IPositionsRepository positionsRepository,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        UpdatePositionDepartmentsCommandValidator validator,
        ILogger<UpdatePositionDepartmentsHandler> logger,
        HybridCache cache)
    {
        _positionsRepository = positionsRepository;
        _departmentsRepository = departmentsRepository;
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

        var position = await _positionsRepository
            .GetBy(x => x.Id == positionId, cancellationToken);

        if (position.IsFailure)
        {
            _logger.LogError("Position is not active!");
            return position.Error.ToErrors();
        }

        var activeDepartmentIdsResult = await _departmentsRepository
            .GetActiveDepartmentIdsAsync(command.PositionDepartmentsRequest.DepartmentsIds, cancellationToken);

        if (activeDepartmentIdsResult.IsFailure)
            return activeDepartmentIdsResult.Error.ToErrors();

        var bulkDeletePositionDepartmentsResult = await _positionsRepository
            .BulkDeletePositionDepartmentsAsync(positionId.Value, cancellationToken);

        if (bulkDeletePositionDepartmentsResult.IsFailure)
        {
            _logger.LogError("Position departments wasn't updated!");
            return bulkDeletePositionDepartmentsResult.Error.ToErrors();
        }

        List<DepartmentPosition> departmentPositions = [];

        foreach (var positionDepartmentsRequest in command.PositionDepartmentsRequest.DepartmentsIds)
        {
            var departmentPosition = DepartmentPosition.Create(
                positionId,
                new DepartmentId(positionDepartmentsRequest));

            if (departmentPosition.IsFailure)
            {
                _logger.LogError("Position departments wasn't updated!");
                return departmentPosition.Error.ToErrors();
            }

            departmentPositions.Add(departmentPosition.Value);
        }

        position.Value.UpdateDepartments(departmentPositions);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync(Constants.POSITION_CACHE_PREFIX, cancellationToken);

        _logger.LogInformation($"Position was updated with id{positionId.Value}");

        return positionId.Value;
    }
}