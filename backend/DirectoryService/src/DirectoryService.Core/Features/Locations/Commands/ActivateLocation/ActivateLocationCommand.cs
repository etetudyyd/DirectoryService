using Core.Abstractions;

namespace DirectoryService.Features.Locations.Commands.ActivateLocation;

public record ActivateLocationCommand(Guid Id) : ICommand;