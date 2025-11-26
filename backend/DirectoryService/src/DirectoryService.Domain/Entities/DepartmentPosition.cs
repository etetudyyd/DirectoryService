using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.ConectionEntitiesVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DevQuestions.Domain.ValueObjects.PositionVO;

namespace DevQuestions.Domain.Entities;

public sealed class DepartmentPosition
{
    public DepartmentPositionId Id { get; init; }

    public DepartmentId DepartmentId { get; init; }

    public PositionId PositionId { get; init; }

    public Department Department { get; private set; } = null!;

    public Position Position { get; private set; } = null!;

    public DepartmentPosition(
        DepartmentPositionId id,
        DepartmentId departmentId,
        PositionId positionId)
    {
        Id = id;
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public DepartmentPosition(
        DepartmentId departmentId,
        PositionId positionId)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public static Result<DepartmentPosition, Error> Create(
        PositionId positionId,
        DepartmentId departmentId)
    {
        if (positionId.Value == Guid.Empty)
        {
            return Error
                .Validation(
                    null,
                    "PositionId is required");
        }

        if (departmentId.Value == Guid.Empty)
        {
            return Error
                .Validation(
                    null,
                    "DepartmentId is required");
        }

        return new DepartmentPosition(
            new DepartmentPositionId(Guid.NewGuid()),
            departmentId,
            positionId);
    }
}