using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService;

public sealed record MediaOwner
{
    private static readonly HashSet<string> AllowedContext =
    [
        "department",
        "location",
        "position"
    ];

    public string Context { get; }

    public Guid EntityId { get; }

    private MediaOwner(string context, Guid entityId)
    {
        Context = context;
        EntityId = entityId;
    }

    public static Result<MediaOwner, Error> Create(string context, Guid entityId)
    {
        if (string.IsNullOrWhiteSpace(context) || context.Length > 50)
            return GeneralErrors.General.ValueIsInvalid(nameof(context));

        string normalizedContext = context.Trim().ToLowerInvariant();
        if (!AllowedContext.Contains(normalizedContext))
            return GeneralErrors.General.ValueIsInvalid(nameof(context));

        if (entityId == Guid.Empty)
            return GeneralErrors.General.ValueIsInvalid(nameof(entityId));

        return new MediaOwner(normalizedContext, entityId);
    }

    public static Result<MediaOwner, Error> ForDepartment(Guid departmentId) => Create("department", departmentId);

    public static Result<MediaOwner, Error> ForLocation(Guid locationId) => Create("location", locationId);

    public static Result<MediaOwner, Error> ForPosition(Guid positionId) => Create("position", positionId);
}