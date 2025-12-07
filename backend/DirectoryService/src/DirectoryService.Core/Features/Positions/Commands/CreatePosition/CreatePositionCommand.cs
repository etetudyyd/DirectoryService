using Core.Abstractions;
using DirectoryService.Positions.Requests;

namespace DirectoryService.Features.Positions.Commands.CreatePosition;

public record CreatePositionCommand(CreatePositionRequest PositionRequest) : ICommand;