namespace DirectoryService.Locations.Dtos;

public record LocationDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string TimeZone { get; init; } = string.Empty;

    public required AddressDto Address { get; set; }

}