using DevQuestions.Web.EndpointResults;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Commands;
using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Application.Features.Departments.Commands.CreateDepartment;
using DirectoryService.Application.Features.Departments.Commands.DeactivateDepartment;
using DirectoryService.Application.Features.Departments.Commands.RelocateDepartmentParent;
using DirectoryService.Application.Features.Departments.Commands.UpdateDepartmentLocations;
using DirectoryService.Application.Features.Departments.Queries;
using DirectoryService.Application.Features.Departments.Queries.GetChildrenDepartments;
using DirectoryService.Application.Features.Departments.Queries.GetRootDepartments;
using DirectoryService.Application.Features.Departments.Queries.GetTopDepartmentsByPositions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Departments.Requests;
using DirectoryService.Contracts.Departments.Responses;
using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DevQuestions.Web.Controllers.Locations;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    [HttpGet("{parentId:guid}")]
    public async Task<EndpointResult<GetChildrenDepartmentsResponse>> GetChildrenDepartments(
        [FromServices] IQueryHandler<GetChildrenDepartmentsResponse, GetChildrenDepartmentsQuery> handler,
        [FromRoute] Guid parentId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetChildrenDepartmentsQuery(
            new GetChildrenDepartmentsRequest(
                parentId,
                page,
                pageSize));

        return await handler.Handle(query, cancellationToken);
    }

    [HttpGet("roots")]
    public async Task<EndpointResult<GetRootDepartmentsResponse>> GetRootDepartments(
        [FromServices] IQueryHandler<GetRootDepartmentsResponse, GetRootDepartmentsQuery> handler,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] int? prefetch,
        CancellationToken cancellationToken)
    {
        var query = new GetRootDepartmentsQuery(
            new GetRootDepartmentsRequest(
                page,
                pageSize,
                prefetch));

        return await handler.Handle(query, cancellationToken);
    }

    [HttpGet("top_positions")]
    public async Task<EndpointResult<GetTopDepartmentsByPositionsResponse>> GetTopDepartmentsByPosition(
        [FromServices] IQueryHandler<GetTopDepartmentsByPositionsResponse,
            GetTopDepartmentsByPositionsQuery> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetTopDepartmentsByPositionsQuery();
        return await handler.Handle(query, cancellationToken);
    }

    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateDepartmentCommand> handler,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(request);

        return await handler.Handle(command, cancellationToken);
    }

    [HttpPut("{departmentId}/locations")]
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
        [FromServices] ICommandHandler<Guid, RelocateDepartmentParentCommand> handler,
        [FromRoute] Guid departmentId,
        [FromBody] RelocateDepartmentParentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RelocateDepartmentParentCommand(departmentId, request);
        return await handler.Handle(command, cancellationToken);
    }

    [Route("{departmentId:Guid}")]
    [HttpDelete]
    public async Task<EndpointResult<Guid>> Deactivate(
        [FromServices] ICommandHandler<Guid, DeactivateDepartmentCommand> handler,
        [FromRoute] Guid departmentId,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateDepartmentCommand(departmentId);
        return await handler.Handle(command, cancellationToken);
    }

}