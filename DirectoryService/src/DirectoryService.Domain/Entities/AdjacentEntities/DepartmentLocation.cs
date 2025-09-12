using DevQuestions.Domain.ValueObjects.DepartmentLocationVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.LocationVO;

namespace DevQuestions.Domain.Entities.AdjacentEntities;

public class DepartmentLocation()
{
    public DepartmentLocationId Id { get; private set; } = null!;

    public DepartmentId DepartmentId { get; private set; } = null!;

    public LocationId LocationId { get; private set; } = null!;

}