using DirectoryService.Application.Services;
using DirectoryService.Contracts.Locations;
using DirectoryService.Presentation.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers.Locations;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] CreateLocationDto locationDto,
        [FromServices] CreateLocationHandler createLocationHandler,
        CancellationToken cancellationToken)
    {
        return await createLocationHandler.Handle(locationDto, cancellationToken);
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] GetLocationDto request,
        CancellationToken cancellationToken)
    {
        return Ok("Location get successfully");
    }

    [HttpGet("{locationId:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        return Ok("Locations get by id successfully");
    }

    [HttpPut("{locationId:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid locationId,
        [FromBody] UpdateLocationDto locationDto,
        CancellationToken cancellationToken)
    {
        return Ok("Location updated successfully");
    }

    [HttpDelete("{locationId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        return Ok("Location deleted successfully");
    }
}