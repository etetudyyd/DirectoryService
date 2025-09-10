using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities.AdjacentEntities;

namespace DevQuestions.Domain.Entities;

public class Position
{
    // ef
    private Position()
    {
    }

    private Position(string name, string description, bool isActive, DateTime createdAt, DateTime updatedAt, List<DepartmentPosition> departmentPositions)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = createdAt;
        _departmentPositions = departmentPositions;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private List<DepartmentPosition> _departmentPositions;

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;

    public static Result<Position> Create(
        string name,
        string description,
        bool isActive,
        DateTime createdAt,
        DateTime updatedAt,
        List<DepartmentPosition> departmentPositions)
    {
        return Result.Success(
            new Position(
                name,
                description,
                isActive,
                createdAt,
                updatedAt,
                departmentPositions));
    }
}