using CSharpFunctionalExtensions;
using DirectoryService.ValueObjects.Location;
using Shared.SharedKernel;

namespace DirectoryService.Entities;

public sealed class Location : ISoftDeletable
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

    public DateTime? DeletedAt { get; private set; }

    public ICollection<DepartmentLocation> DepartmentLocations => _departmentLocations;

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
        DeletedAt = null;
        IsActive = true;
        _departmentLocations = departmentLocations.ToList();
    }

    public static Result<Location, Error> Create(
        LocationId id,
        LocationName name,
        Address address,
        Timezone timezone,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        var departmentLocationsList = departmentLocations.ToList();

        if (string.IsNullOrWhiteSpace(name.Value)
            || name.Value.Length > Constants.MAX_LENGTH_LOCATION_NAME)
        {
            return Error
                .Validation(
                    null,
                    "Name is required and must be less than 150 characters");
        }

        return new Location(
            id,
            name,
            address,
            timezone,
            departmentLocationsList);
    }

    public UnitResult<Error> Deactivate()
    {
        if(!IsActive)
            return Error.Failure("location.error.delete", "location is already not active");
        IsActive = false;
        DeletedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Activate()
    {
        if(IsActive)
            return Error.Failure("location.error.delete", "location is already active");
        IsActive = true;

        return UnitResult.Success<Error>();
    }
}