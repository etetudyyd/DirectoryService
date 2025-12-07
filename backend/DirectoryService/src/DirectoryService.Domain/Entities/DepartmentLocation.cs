using CSharpFunctionalExtensions;
using DirectoryService.ValueObjects.ConnectionEntities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Location;
using Shared.SharedKernel;

namespace DirectoryService.Entities;

public sealed class DepartmentLocation
{
    public DepartmentLocationId Id { get; init; }

    public DepartmentId DepartmentId { get; init; }

    public LocationId LocationId { get; init; }

    public Department Department { get; private set; } = null!;

    public Location Location { get; private set; } = null!;

    public DepartmentLocation(
        DepartmentLocationId id,
        DepartmentId departmentId,
        LocationId locationId)
    {
        Id = id;
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public DepartmentLocation(
        DepartmentId departmentId,
        LocationId locationId)
    {
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public static Result<DepartmentLocation, Error> Create(
        LocationId locationId,
        DepartmentId departmentId)
    {
        if (locationId.Value == Guid.Empty)
        {
            return Error
                .Validation(
                    null,
                    "LocationId is required");
        }

        if (departmentId.Value == Guid.Empty)
        {
            return Error
                .Validation(
                    null,
                    "DepartmentId is required");
        }

        return new DepartmentLocation(
            new DepartmentLocationId(Guid.NewGuid()),
            departmentId,
            locationId);
    }
}