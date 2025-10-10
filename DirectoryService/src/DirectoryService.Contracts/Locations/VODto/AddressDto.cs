namespace DirectoryService.Contracts.Locations.VODto;

public record AddressDto(
    string PostalCode,
    string Region,
    string City,
    string Street,
    string House,
    string? Apartment);