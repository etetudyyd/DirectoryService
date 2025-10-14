namespace DirectoryService.Contracts.Locations.Requests;

public record LocationDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;

    public string TimeZone { get; init; } = string.Empty;

}
