namespace DevQuestions.Domain.Entities.AdjacentEntities;

public class DepartmentPosition(Department department, Position position)
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Department Department { get; private set; } = department;

    public Position Position { get; private set; } = position;
}