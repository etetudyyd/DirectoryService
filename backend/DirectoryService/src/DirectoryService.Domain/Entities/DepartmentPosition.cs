using CSharpFunctionalExtensions;
using DirectoryService.ValueObjects.ConnectionEntities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Position;
using Shared.SharedKernel;

namespace DirectoryService.Entities;

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