using Core.Abstractions;

namespace DirectoryService.Features.Positions.Commands.DeactivatePosition;

public record DeactivatePositionCommand(Guid Id) : ICommand;