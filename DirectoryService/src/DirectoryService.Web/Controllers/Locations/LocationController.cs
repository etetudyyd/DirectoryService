using DevQuestions.Domain.Entities;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DevQuestions.Web.EndpointResults;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Application.CQRS.Locations.Commands.CreateLocation;
using DirectoryService.Application.CQRS.Locations.Queries.GetLocations;
using DirectoryService.Contracts.Locations.Requests;
using DirectoryService.Contracts.Locations.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DevQuestions.Web.Controllers.Locations;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler,
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request);

        return await handler.Handle(command, cancellationToken);
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