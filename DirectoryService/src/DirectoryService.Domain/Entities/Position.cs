using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.PositionVO;

namespace DevQuestions.Domain.Entities;

public class Position
{
    // ef
    private Position()
    {
    }

    private Position(Guid id, PositionName name, PositionDescription description, bool isActive, DateTime createdAt, DateTime updatedAt, List<DepartmentPosition> departmentPositions)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = createdAt;
        _departmentPositions = departmentPositions;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; private set; }

    public PositionName Name { get; private set; }

    public PositionDescription Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private List<DepartmentPosition> _departmentPositions;

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;

    public static Result<Position, Error> Create(
        PositionName name,
        PositionDescription description,
        bool isActive,
        DateTime createdAt,
        DateTime updatedAt,
        List<DepartmentPosition> departmentPositions)
    {
        return new Position(
            Guid.NewGuid(),
            name,
            description,
            isActive,
            createdAt,
            updatedAt,
            departmentPositions);
    }
}