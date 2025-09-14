namespace DevQuestions.Domain.Entities;

public class DepartmentPosition
{
    public Guid Id { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid PositionId { get; private set; }
}