namespace DirectoryService.Contracts.Locations.Dtos;

public record AddressDto(
    string PostalCode,
    string Region,
    string City,
    string Street,
    string House,
    string? Apartment);