using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.PositionVO;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Application.Extentions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Features.Positions.CreatePosition;

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

        var name = PositionName.Create(command.PositionDto.Name.Value);
        if (name.IsFailure)
        {
            _logger.LogError("Invalid PositionDto.Name");
            return name.Error.ToErrors();
        }

        var description = Description.Create(command.PositionDto.Description.Value);

        var position = Position.Create(
            name.Value,
            description.Value,
            command.PositionDto.DepartmentPositions);

        if (position.IsFailure)
        {
            _logger.LogError("Invalid PositionDto.Position");
            return position.Error.ToErrors();
        }


        var positionId = await _positionsRepository.AddAsync(position.Value, cancellationToken);

        _logger.LogInformation($"Location created successfully with id {positionId}", positionId);

        return positionId;
    }
}