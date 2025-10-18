﻿using DevQuestions.Web.EndpointResults;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Application.Features.Locations.Commands.CreateLocation;
using DirectoryService.Application.Features.Locations.Queries.GetLocationById;
using DirectoryService.Application.Features.Locations.Queries.GetLocations;
using DirectoryService.Contracts.Locations.Requests;
using DirectoryService.Contracts.Locations.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DevQuestions.Web.Controllers.Locations;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost("create")]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler,
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request);

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
}