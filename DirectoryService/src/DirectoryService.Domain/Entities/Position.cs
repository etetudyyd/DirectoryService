using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DevQuestions.Domain.ValueObjects.PositionVO;

namespace DevQuestions.Domain.Entities;

public sealed class Position : ISoftDeletable
{
    // ef
    private Position() { }

    private List<DepartmentPosition> _departmentPositions;

    public PositionId Id { get; private set; }

    public PositionName Name { get; private set; }

    public Description Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public ICollection<DepartmentPosition> DepartmentPositions => _departmentPositions;

    private Position(
        PositionId id,
        PositionName name,
        Description description,
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
        PositionName name,
        Description description,
        IEnumerable<DepartmentPosition> departmentPositions)
    {
        if (string.IsNullOrWhiteSpace(name.Value) || name.Value.Length > Constants.MAX_LENGTH_POSITION_NAME)
        {
            return Error.Validation(
                null,
                "Name is required and must be less than 150 characters");
        }

        return new Position(
            new PositionId(Guid.NewGuid()),
            name,
            description,
            departmentPositions);
    }

    public UnitResult<Error> Delete()
    {
        if(!IsActive)
            return Error.Failure("position.error.delete", "position is already not active");
        IsActive = false;
        DeletedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Restore()
    {
        if(IsActive)
            return Error.Failure("position.error.delete", "position is already active");
        IsActive = true;

        return UnitResult.Success<Error>();
    }
}