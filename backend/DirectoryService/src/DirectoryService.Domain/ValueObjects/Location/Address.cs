using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService.ValueObjects.Location;

public record Address
{
    public string PostalCode { get; }

    public string Region { get; }

    public string City { get; }

    public string Street { get; }

    public string House { get; }

    public string? Apartment { get; }

    private Address(string postalCode, string region, string city, string street, string house, string? apartment)
    {
        PostalCode = postalCode;
        Region = region;
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
    }

    public static Result<Address, Error> Create(string postalCode, string region,
        string city, string street, string house, string? apartment = null)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return Error.Validation(null, "Index is required.");
        if (!Regex.IsMatch(postalCode, @"^\d{6}"))
            return Error.Validation(null, "Postal code must be 6 digits.");

        if (string.IsNullOrWhiteSpace(region))
            return Error.Validation(null, "Region is required.");

        if (string.IsNullOrWhiteSpace(city))
            return Error.Validation(null, "City is required.");

        if (string.IsNullOrWhiteSpace(street))
            return Error.Validation(null, "Street is required.");

        if (string.IsNullOrWhiteSpace(house))
            return Error.Validation(null, "House is required.");

        return new Address(
            postalCode.Trim(),
            region.Trim(),
            city.Trim(),
            street.Trim(),
            house.Trim(),
            apartment?.Trim());
    }

    public override string ToString()
    {
        return $"{PostalCode}, {Region}, c. {City}, st. {Street}, h. {House}"
               + (string.IsNullOrEmpty(Apartment) ? string.Empty : $", ap. {Apartment}");
    }

}