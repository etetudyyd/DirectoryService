using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.LocationVO;
using Guid = System.Guid;

namespace DevQuestions.Domain.Entities;

public sealed class Location
{
    // ef
    private Location() { }

    private readonly List<DepartmentLocation> _departmentLocations = [];

    public LocationId Id { get; private set; }

    public LocationName Name { get; private set; }

    public Address Address { get; private set; }

    public Timezone Timezone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    private Location(
        LocationId id,
        LocationName name,
        Address address,
        Timezone timezone,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        Id = id;
        Name = name;
        Address = address;
        Timezone = timezone;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
        _departmentLocations = departmentLocations.ToList();
    }

    public static Result<Location, Error> Create(
        LocationName name,
        Address address,
        Timezone timezone,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        var departmentLocationsList = departmentLocations.ToList();

        if (string.IsNullOrWhiteSpace(name.Value)
            || name.Value.Length > LengthConstants.MAX_LENGTH_LOCATION_NAME)
        {
            return Error
                .Validation(
                    null,
                    "Name is required and must be less than 150 characters");
        }

        return new Location(
            new LocationId(Guid.NewGuid()),
            name,
            address,
            timezone,
            departmentLocationsList);
    }
}