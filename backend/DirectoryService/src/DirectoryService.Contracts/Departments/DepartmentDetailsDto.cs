using DirectoryService.Locations;
using DirectoryService.Positions;

namespace DirectoryService.Departments;

public record DepartmentDetailsDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public Guid? ParentId { get; init; }

    public int Depth { get; init; }

    public bool IsActive { get; init; }

    public int LocationsCount { get; init; }

    public int PositionsCount { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public DateTime? DeletedAt { get; init; }

    public IReadOnlyList<LocationItemDto> Locations { get; init; } = [];

    public IReadOnlyList<PositionItemDto> Positions { get; init; } = [];
}