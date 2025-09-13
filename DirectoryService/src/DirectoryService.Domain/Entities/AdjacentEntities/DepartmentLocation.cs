using DevQuestions.Domain.ValueObjects.DepartmentLocationVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.LocationVO;

namespace DevQuestions.Domain.Entities.AdjacentEntities;

public class DepartmentLocation()
{
    public Guid Id { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid LocationId { get; private set; }

}