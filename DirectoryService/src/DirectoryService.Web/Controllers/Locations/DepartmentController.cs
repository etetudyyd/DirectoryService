using Microsoft.AspNetCore.Mvc;

namespace DevQuestions.Web.Controllers.Locations;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        return Ok("Location get successfully");
    }

}