using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Features.Locations.CreateLocation;

public record CreateLocationCommand(CreateLocationDto LocationDto) : ICommand;