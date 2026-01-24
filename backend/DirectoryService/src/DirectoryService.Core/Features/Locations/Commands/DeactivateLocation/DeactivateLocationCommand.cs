using Core.Abstractions;

namespace DirectoryService.Features.Locations.Commands.DeactivateLocation;

public record DeactivateLocationCommand(Guid Id) : ICommand;