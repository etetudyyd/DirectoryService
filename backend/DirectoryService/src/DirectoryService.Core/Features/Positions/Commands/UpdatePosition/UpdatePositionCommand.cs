using Core.Abstractions;
using DirectoryService.Positions.Requests;

namespace DirectoryService.Features.Positions.Commands.UpdatePosition;

public record UpdatePositionCommand(Guid Id, UpdatePositionRequest PositionRequest) : ICommand;