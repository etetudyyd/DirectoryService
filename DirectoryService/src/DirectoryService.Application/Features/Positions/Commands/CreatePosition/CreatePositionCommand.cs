using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Contracts.Positions.Requests;

namespace DirectoryService.Application.Features.Positions.Commands.CreatePosition;

public record CreatePositionCommand(CreatePositionRequest PositionRequest) : ICommand;