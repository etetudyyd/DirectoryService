using Core.Abstractions;
using DirectoryService.Positions.Requests;

namespace DirectoryService.Features.Positions.Commands.UpdatePositionDepartments;

public record UpdatePositionDepartmentsCommand(Guid PositionId, UpdatePositionDepartmentsRequest PositionDepartmentsRequest) : ICommand;