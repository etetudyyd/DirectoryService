using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Contracts.Locations.Requests;

namespace DirectoryService.Application.Features.Locations.Commands.CreateLocation;

public record CreateLocationCommand(CreateLocationRequest LocationRequest) : ICommand;