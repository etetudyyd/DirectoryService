using CSharpFunctionalExtensions;

namespace DevQuestions.Domain.Entities;

public class Position
{
    // ef
    private Position()
    {
    }

    private Position(string name, string description, bool isActive, DateTime createdAt, DateTime updatedAt)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static Result<Position> Create(string name, string description, bool isActive, DateTime createdAt, DateTime updatedAt)
    {
        return Result.Success(new Position(name, description, isActive, createdAt, updatedAt));
    }
}