using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Locations.Requests;

namespace DirectoryService.Application.CQRS.Locations.Commands.CreateLocation;

public record CreateLocationCommand(CreateLocationRequest LocationRequest) : ICommand;