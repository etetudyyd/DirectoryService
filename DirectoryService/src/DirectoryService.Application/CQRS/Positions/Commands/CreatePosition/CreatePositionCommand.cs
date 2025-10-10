using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Contracts.Positions;
using DirectoryService.Contracts.Positions.Requests;

namespace DirectoryService.Application.CQRS.Positions.Commands.CreatePosition;

public record CreatePositionCommand(CreatePositionRequest PositionRequest) : ICommand;