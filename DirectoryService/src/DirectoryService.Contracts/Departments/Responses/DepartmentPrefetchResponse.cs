namespace DirectoryService.Contracts.Departments.Responses;

public sealed record DepartmentPrefetchResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public Guid? ParentId { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdateAt { get; init; }

    public bool HasMoreChildren { get; init; }

    public List<DepartmentPrefetchResponse> Children { get; init; } = [];

    /*c.id,
    c.name,
    c.identifier,
    c.path,
    c.parent_id,
    c.is_active,
    c.created_at,
    c.update_at,
    c.has_more_children*/
}
