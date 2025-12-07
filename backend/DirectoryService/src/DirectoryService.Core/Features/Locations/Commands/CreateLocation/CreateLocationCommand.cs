using Core.Abstractions;
using DirectoryService.Locations.Requests;

namespace DirectoryService.Features.Locations.Commands.CreateLocation;

public record CreateLocationCommand(CreateLocationRequest LocationRequest) : ICommand;