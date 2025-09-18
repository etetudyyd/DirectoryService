using DevQuestions.Domain.ValueObjects.ConectionEntitiesVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.LocationVO;

namespace DevQuestions.Domain.Entities;

public sealed class DepartmentLocation
{
    public DepartmentLocationId Id { get; init; }

    public DepartmentId DepartmentId { get; init; }

    public LocationId LocationId { get; init; }

}