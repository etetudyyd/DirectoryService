using DevQuestions.Domain.Entities;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DevQuestions.Web.EndpointResults;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Features.Locations.CreateLocation;
using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DevQuestions.Web.Controllers.Locations;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler,
        [FromBody] CreateLocationDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request);

        return await handler.Handle(command, cancellationToken);
    }
}