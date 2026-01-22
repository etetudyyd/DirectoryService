using Core.Abstractions;
using DirectoryService.Locations.Requests;

namespace DirectoryService.Features.Locations.Commands.UpdateLocation;

public record UpdateLocationCommand(Guid Id, UpdateLocationRequest LocationRequest) : ICommand;