using Core.Abstractions;
using DirectoryService.Entities;
using DirectoryService.Features.Positions.Commands.CreatePosition;
using DirectoryService.Features.Positions.Commands.DeactivatePosition;
using DirectoryService.Features.Positions.Commands.UpdatePosition;
using DirectoryService.Features.Positions.Queries.GetPositionById;
using DirectoryService.Features.Positions.Queries.GetPositions;
using DirectoryService.Positions.Requests;
using DirectoryService.Positions.Responses;
using Framework.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreatePositionCommand> handler,
        [FromBody] CreatePositionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpGet]
    public async Task<EndpointResult<GetPositionsResponse>> Get(
        [FromServices] IQueryHandler<GetPositionsResponse, GetPositionsQuery> handler,
        [FromQuery] GetPositionsRequest request,
        CancellationToken cancellationToken)
    {
        GetPositionsQuery query = new(request);
        return await handler.Handle(query, cancellationToken);
    }

    [HttpGet("{positionId:guid}")]
    public async Task<EndpointResult<GetPositionByIdResponse>> GetById(
        [FromServices] IQueryHandler<GetPositionByIdResponse, GetPositionByIdQuery> handler,
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        GetPositionByIdQuery query = new(positionId);

        return await handler.Handle(query, cancellationToken);
    }

    [HttpPatch("{positionId}")]
    public async Task<EndpointResult<Position>> Update(
        [FromServices] ICommandHandler<Position, UpdatePositionCommand> handler,
        [FromRoute] Guid positionId,
        [FromBody] UpdatePositionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePositionCommand(positionId, request);

        return await handler.Handle(command, cancellationToken);
    }

    [Route("{positionId:Guid}")]
    [HttpDelete]
    public async Task<EndpointResult<Guid>> Deactivate(
        [FromServices] ICommandHandler<Guid, DeactivatePositionCommand> handler,
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        var command = new DeactivatePositionCommand(positionId);
        return await handler.Handle(command, cancellationToken);
    }
}