using Core.Abstractions;
using CSharpFunctionalExtensions;
using DirectoryService.Database;
using DirectoryService.Departments;
using DirectoryService.Departments.Responses;
using DirectoryService.Locations;
using DirectoryService.Positions;
using DirectoryService.ValueObjects.Department;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Queries.GetDepartment;

public class GetDepartmentHandler : IQueryHandler<GetDepartmentByIdResponse, GetDepartmentQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ILogger<GetDepartmentHandler> _logger;
    private readonly HybridCache _cache;

    public GetDepartmentHandler(
        ILogger<GetDepartmentHandler> logger,
        HybridCache cache,
        IReadDbContext readDbContext)
    {
        _logger = logger;
        _cache = cache;
        _readDbContext = readDbContext;
    }

    public async Task<Result<GetDepartmentByIdResponse, Errors>> Handle(GetDepartmentQuery query, CancellationToken cancellationToken)
    {
        var departmentDto = await _readDbContext.DepartmentsRead
            .Where(d => d.Id == new DepartmentId(query.DepartmentId))
            .Select(d => new DepartmentDetailsDto
            {
                Id = d.Id.Value,
                Name = d.Name.Value,
                Identifier = d.Identifier.Value,
                Path = d.Path.Value,
                ParentId = d.ParentId!.Value,
                Depth = d.Depth,
                IsActive = d.IsActive,
                LocationsCount = d.DepartmentLocations.Count,
                PositionsCount = d.DepartmentPositions.Count,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                DeletedAt = d.DeletedAt,
                Positions = d.DepartmentPositions
                    .Where(dp => dp.Position.DeletedAt == null)
                    .Select(dp => new PositionItemDto
                    {
                        Id = dp.Position.Id.Value,
                        Name = dp.Position.Name.Value,
                        Description = dp.Position.Description.Value,
                        IsActive = dp.Position.IsActive,
                        CreatedAt = dp.Position.CreatedAt,
                        UpdatedAt = dp.Position.UpdatedAt,
                        DeletedAt = dp.Position.DeletedAt,
                    })
                    .ToList(),
                Locations = d.DepartmentLocations
                    .Where(dl => dl.Location.DeletedAt == null)
                    .Select(dl => new LocationItemDto
                    {
                        Id = dl.Location.Id.Value,
                        Name = dl.Location.Name.Value,
                        Address = new AddressDto
                        {
                            Apartment = dl.Location.Address.Apartment,
                            City = dl.Location.Address.City,
                            House = dl.Location.Address.House,
                            PostalCode = dl.Location.Address.PostalCode,
                            Street = dl.Location.Address.Street,
                            Region = dl.Location.Address.Region,
                        },
                        TimeZone = dl.Location.Timezone.Value,
                        IsActive = dl.Location.IsActive,
                        CreatedAt = dl.Location.CreatedAt,
                        UpdatedAt = dl.Location.UpdatedAt,
                        DeletedAt = dl.Location.DeletedAt,
                    })
                .ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (departmentDto is null)
        {
            return Error.NotFound("department.not.found", "Department not found")
                .ToErrors();
        }

        _logger.LogInformation("Get department by id: {Id}", departmentDto.Id);

        return new GetDepartmentByIdResponse(departmentDto);
    }
}