namespace DirectoryService.Locations;

public record AddressDto {
    public string PostalCode { get;  init; } = string.Empty;

    public string Region { get;  init; } = string.Empty;

    public string City { get;  init; } = string.Empty;

    public string Street { get;  init; } = string.Empty;

    public string House { get;  init; } = string.Empty;

    public string? Apartment { get;  init; } = string.Empty;

}