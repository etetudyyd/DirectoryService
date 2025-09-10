using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities.AdjacentEntities;
using DevQuestions.Domain.ValueObjects.LocationVO;

namespace DevQuestions.Domain.Entities;

public class Location
{
    private Location(
        string name,
        Address address,
        Timezone timezone,
        bool isActive,
        DateTime createdAt,
        List<DepartmentLocation> departmentLocations,
        List<Department> departments)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        Timezone = timezone;
        IsActive = isActive;
        CreatedAt = createdAt;
        _departmentLocations = departmentLocations;
        _departments = departments;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public Address Address { get; private set; }

    public Timezone Timezone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private List<DepartmentLocation> _departmentLocations;

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    private List<Department> _departments;

    public List<Department> Departments => _departments;

    public static Result<Location> Create(string name, Address address, Timezone timezone, bool isActive,
        DateTime createdAt, List<DepartmentLocation> departmentLocations, List<Department> departments)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 150)
        {
            return Result.Failure<Location>("Name is required and must be less than 150 characters");
        }

        return new Location(name, address, timezone, isActive, createdAt, departmentLocations, departments);
    }
}