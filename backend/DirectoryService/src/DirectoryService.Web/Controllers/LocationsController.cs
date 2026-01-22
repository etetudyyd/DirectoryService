using Core.Abstractions;
using DirectoryService.Entities;
using DirectoryService.Features.Locations.Commands.CreateLocation;
using DirectoryService.Features.Locations.Commands.DeactivateLocation;
using DirectoryService.Features.Locations.Commands.UpdateLocation;
using DirectoryService.Features.Locations.Queries.GetLocationById;
using DirectoryService.Features.Locations.Queries.GetLocations;
using DirectoryService.Locations.Dtos;
using DirectoryService.Locations.Requests;
using DirectoryService.Locations.Responses;
using Framework.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler,
        [FromBody] LocationDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(
            new CreateLocationRequest(
                new NameDto(request.Name),
                request.Address,
                new TimezoneDto(request.TimeZone),
                request.DepartmentIds));

        return await handler.Handle(command, cancellationToken);
    }

    [HttpGet("{locationId:guid}")]
    public async Task<EndpointResult<GetLocationByIdResponse>> GetById(
        [FromServices] IQueryHandler<GetLocationByIdResponse, GetLocationByIdQuery> handler,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        GetLocationByIdQuery query = new(locationId);

        return await handler.Handle(query, cancellationToken);
    }

    [HttpGet]
    public async Task<EndpointResult<GetLocationsResponse>> Get(
        [FromServices] IQueryHandler<GetLocationsResponse, GetLocationsQuery> handler,
        [FromQuery] GetLocationsRequest request,
        CancellationToken cancellationToken)
    {
        GetLocationsQuery query = new(request);
        return await handler.Handle(query, cancellationToken);
    }

    [HttpPatch("{locationId}")]
    public async Task<EndpointResult<Location>> Update(
        [FromServices] ICommandHandler<Location, UpdateLocationCommand> handler,
        [FromRoute] Guid locationId,
        [FromBody] LocationDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand(
            locationId,
            new UpdateLocationRequest(
                new NameDto(request.Name),
                request.Address,
                new TimezoneDto(request.TimeZone),
                request.DepartmentIds));

        return await handler.Handle(command, cancellationToken);
    }

    [Route("{locationId:Guid}")]
    [HttpDelete]
    public async Task<EndpointResult<Guid>> Deactivate(
        [FromServices] ICommandHandler<Guid, DeactivateLocationCommand> handler,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateLocationCommand(locationId);
        return await handler.Handle(command, cancellationToken);
    }

}