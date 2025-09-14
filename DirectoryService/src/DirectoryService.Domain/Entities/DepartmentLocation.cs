namespace DevQuestions.Domain.Entities;

public class DepartmentLocation()
{
    public Guid Id { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid LocationId { get; private set; }

}