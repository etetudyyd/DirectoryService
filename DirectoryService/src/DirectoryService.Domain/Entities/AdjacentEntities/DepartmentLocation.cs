namespace DevQuestions.Domain.Entities.AdjacentEntities;

public class DepartmentLocation(Department department, Location location)
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Department Department { get; private set; } = department;

    public Location Location { get; private set; } = location;
}