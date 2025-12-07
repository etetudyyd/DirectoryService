namespace DirectoryService.Departments.Responses;

public record DepartmentResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public Guid? ParentId { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdateAt { get; init; }

    public int PositionCount { get; init; }
}
