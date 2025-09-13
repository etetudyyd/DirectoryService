using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities.AdjacentEntities;
using DevQuestions.Domain.ValueObjects.LocationVO;

namespace DevQuestions.Domain.Entities;

public class Location
{
    // ef
    private Location() { }

    private Location(
        LocationId id,
        LocationName name,
        Address address,
        Timezone timezone,
        DateTime createdAt,
        bool isActive,
        List<DepartmentLocation> departmentLocations
        )
    {
        Id = id;
        Name = name;
        Address = address;
        Timezone = timezone;
        CreatedAt = createdAt;
        IsActive = isActive;
        _departmentLocations = departmentLocations;
    }

    public LocationId Id { get; private set; }

    public LocationName Name { get; private set; }

    public Address Address { get; private set; }

    public Timezone Timezone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private List<DepartmentLocation> _departmentLocations;

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public static Result<Location> Create(LocationName name, Address address, Timezone timezone,
        DateTime createdAt, bool isActive, List<DepartmentLocation> departmentLocations)
    {
        if (string.IsNullOrWhiteSpace(name.Value) || name.Value.Length > 150)
        {
            return Result.Failure<Location>("Name is required and must be less than 150 characters");
        }

        return new Location(new LocationId(Guid.NewGuid()), name, address, timezone, createdAt, isActive, departmentLocations);
    }
}