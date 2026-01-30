using CSharpFunctionalExtensions;
using DirectoryService.ValueObjects.Position;
using Shared.SharedKernel;

namespace DirectoryService.Entities;

public sealed class Position : ISoftDeletable
{
    // ef
    private Position() { }

    private List<DepartmentPosition> _departmentPositions;

    public PositionId Id { get; private set; }

    public PositionName Name { get; private set; }

    public PositionDescription Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public ICollection<DepartmentPosition> DepartmentPositions => _departmentPositions;

    private Position(
        PositionId id,
        PositionName name,
        PositionDescription description,
        IEnumerable<DepartmentPosition> departmentPositions)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DeletedAt = null;
        _departmentPositions = departmentPositions.ToList();
    }

    public static Result<Position, Error> Create(
        PositionId id,
        PositionName name,
        PositionDescription positionDescription,
        IEnumerable<DepartmentPosition> departmentPositions)
    {
        if (string.IsNullOrWhiteSpace(name.Value) || name.Value.Length > Constants.MAX_LENGTH_POSITION_NAME)
        {
            return Error.Validation(
                null,
                "Name is required and must be less than 150 characters");
        }

        return new Position(
            id,
            name,
            positionDescription,
            departmentPositions);
    }

    public void AddDepartments(IEnumerable<DepartmentPosition> departmentPosition)
    {
        var departmentPositions = departmentPosition
            .Select(p => DepartmentPosition.Create(p.PositionId, p.DepartmentId).Value);
        _departmentPositions.AddRange(departmentPositions);
    }

    public void UpdateDepartments(IEnumerable<DepartmentPosition> departmentPositions)
    {
        _departmentPositions.Clear();
        AddDepartments(departmentPositions);
    }

    public UnitResult<Error> Update(PositionName name, PositionDescription description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Deactivate()
    {
        if(!IsActive)
            return Error.Failure("position.error.delete", "position is already not active");
        IsActive = false;
        DeletedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Activate()
    {
        if(IsActive)
            return Error.Failure("position.error.delete", "position is already active");
        IsActive = true;

        return UnitResult.Success<Error>();
    }
}