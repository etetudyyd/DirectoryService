using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IRepositories;
using DirectoryService.Entities;
using DirectoryService.ValueObjects.ConnectionEntities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Position;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Commands.CreatePosition;

public class CreatePositionHandler: ICommandHandler<Guid, CreatePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;

    private readonly ILogger<CreatePositionHandler> _logger;

    private readonly IValidator<CreatePositionCommand> _validator;

    public CreatePositionHandler(
        IPositionsRepository positionsRepository,
        ILogger<CreatePositionHandler> logger,
        IValidator<CreatePositionCommand> validator)
    {
        _positionsRepository = positionsRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(
        CreatePositionCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Invalid PositionDto");
            return validationResult.ToErrors();
        }

        var name = PositionName.Create(command.PositionRequest.Name);
        if (name.IsFailure)
        {
            _logger.LogError("Invalid PositionDto.Name");
            return name.Error.ToErrors();
        }

        var description = Description.Create(command.PositionRequest.Description);

        var positionId = new PositionId(Guid.NewGuid());

        var departmentPositions = command.PositionRequest.DepartmentsIds
            .Select(departmentId => new DepartmentPosition(
                new DepartmentPositionId(Guid.NewGuid()),
                new DepartmentId(departmentId),
                positionId))
            .ToList();

        var position = Position.Create(
            positionId,
            name.Value,
            description.Value,
            departmentPositions);

        if (position.IsFailure)
        {
            _logger.LogError("Invalid PositionDto.Position");
            return position.Error.ToErrors();
        }


        await _positionsRepository.AddAsync(position.Value, cancellationToken);

        _logger.LogInformation($"Location created successfully with id {positionId}", positionId);

        return positionId.Value;
    }
}