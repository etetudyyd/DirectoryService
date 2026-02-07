namespace DirectoryService.Positions;

public record PositionDetailsDto
{

    public PositionDetailsDto()
    {
        Departments = [];
    }

    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public int DepartmentCount { get; set; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public DateTime? DeletedAt { get; init; }

    public List<DictionaryItemResponse> Departments { get; init; } = [];
}