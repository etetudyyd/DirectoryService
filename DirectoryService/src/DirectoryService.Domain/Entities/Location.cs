using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.LocationVO;
using Guid = System.Guid;

namespace DevQuestions.Domain.Entities;

public class Location
{
    // ef
    private Location() { }

    private Location(
        Guid id,
        LocationName name,
        Address address,
        Timezone timezone,
        DateTime createdAt,
        bool isActive
       /* List<DepartmentLocation> departmentLocations*/)
    {
        Id = id;
        Name = name;
        Address = address;
        Timezone = timezone;
        CreatedAt = createdAt;
        IsActive = isActive;
        //_departmentLocations = departmentLocations;
    }

    public Guid Id { get; private set; }

    public LocationName Name { get; private set; }

    public Address Address { get; private set; }

    public Timezone Timezone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private List<DepartmentLocation> _departmentLocations;

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public static Result<Location, Error> Create(LocationName name, Address address, Timezone timezone,
        DateTime createdAt, bool isActive)//, List<DepartmentLocation> departmentLocations)
    {
        if (string.IsNullOrWhiteSpace(name.Value) || name.Value.Length > 150)
        {
            return Error.Validation(null, "Name is required and must be less than 150 characters");
        }

        return new Location(Guid.NewGuid(), name, address, timezone, createdAt, isActive/*, departmentLocations*/);
    }
}