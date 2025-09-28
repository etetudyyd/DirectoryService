using System.Net;
using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.ConectionEntitiesVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.LocationVO;

namespace DevQuestions.Domain.Entities;

public sealed class DepartmentLocation
{
    public DepartmentLocationId Id { get; init; }

    public DepartmentId DepartmentId { get; init; }

    public LocationId LocationId { get; init; }

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