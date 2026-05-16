using Core.Abstractions;

namespace DirectoryService.Features.Positions.Commands.ActivatePosition;

public record ActivatePositionCommand(Guid Id) : ICommand;