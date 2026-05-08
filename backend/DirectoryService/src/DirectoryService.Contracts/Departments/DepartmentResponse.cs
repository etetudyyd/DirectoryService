namespace DirectoryService.Departments;

public record DepartmentResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public Guid? ParentId { get; init; }

    public int Depth { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public DateTime? DeletedAt { get; init; }

    //public IReadOnlyList<DepartmentItemDto> ChildrenDepartments { get; init; }

    //public IReadOnlyList<DepartmentLocation> DepartmentLocations { get; init; }

    //public IReadOnlyList<DepartmentPosition> DepartmentPositions { get; init; }
}