namespace DirectoryService.Locations;

public record LocationItemDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string TimeZone { get; init; } = string.Empty;

    public required AddressDto Address { get; set; }

    public bool IsActive { get; init; }

    public int DepartmentCount { get; set; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public DateTime? DeletedAt { get; init; }
}