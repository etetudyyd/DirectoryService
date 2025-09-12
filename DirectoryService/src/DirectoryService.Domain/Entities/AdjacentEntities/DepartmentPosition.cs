using DevQuestions.Domain.ValueObjects.DepartmentPositionVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.PositionVO;

namespace DevQuestions.Domain.Entities.AdjacentEntities;

public class DepartmentPosition
{
    public DepartmentPositionId Id { get; private set; } = null!;

    public DepartmentId DepartmentId { get; private set; } = null!;

    public PositionId PositionId { get; private set; } = null!;
}