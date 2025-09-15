using DirectoryService.Application.Services;
using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers.Locations;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLocationDto locationDto,
        [FromServices] CreateLocationHandler createLocationHandler,
        CancellationToken cancellationToken)
    {
        var result = await createLocationHandler.Handle(locationDto, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);

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