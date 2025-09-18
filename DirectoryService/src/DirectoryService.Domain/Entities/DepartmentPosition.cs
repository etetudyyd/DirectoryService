using DevQuestions.Domain.ValueObjects.ConectionEntitiesVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.PositionVO;

namespace DevQuestions.Domain.Entities;

public sealed class DepartmentPosition
{
    public DepartmentPositionId Id { get; init; }

    public DepartmentId DepartmentId { get; init; }

    public PositionId PositionId { get; init; }
}