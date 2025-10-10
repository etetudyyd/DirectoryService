using DevQuestions.Web.EndpointResults;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.CQRS.Departments.Commands.CreateDepartment;
using DirectoryService.Application.CQRS.Departments.Commands.RelocateDepartmentParent;
using DirectoryService.Application.CQRS.Departments.Commands.UpdateDepartmentLocations;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Departments.Requests;
using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DevQuestions.Web.Controllers.Locations;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateDepartmentCommand> handler,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpPut("/api/departments/{departmentId}/locations")]
    public async Task<EndpointResult<Guid>> UpdateDepartmentLocations(
        [FromServices] ICommandHandler<Guid, UpdateDepartmentLocationsCommand> handler,
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentLocationsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentLocationsCommand(departmentId, request);
        return await handler.Handle(command, cancellationToken);
    }

    [Route("{departmentId:Guid}/parent")]
    [HttpPut]
    public async Task<EndpointResult<Guid>> RelocateDepartmentParent(
        [FromServices] RelocateDepartmentParentHandler handler,
        [FromRoute] Guid departmentId,
        [FromBody] RelocateDepartmentParentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RelocateDepartmentParentCommand(departmentId, request);
        return await handler.Handle(command, cancellationToken);
    }

}