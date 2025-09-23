using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Positions;

namespace DirectoryService.Application.Features.Positions.CreatePosition;

public record CreatePositionCommand(CreatePositionDto PositionDto) : ICommand;