using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DevQuestions.Domain.ValueObjects.PositionVO;

namespace DevQuestions.Domain.ValueObjects.LocationVO;

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

    public static Result<Address> Create(string postalCode, string region,
        string city, string street, string house, string? apartment = null)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return Result.Failure<Address>("Index is required.");
        if (!Regex.IsMatch(postalCode, @"^\d{6}"))
            return Result.Failure<Address>("Postal code must be 6 digits.");

        if (string.IsNullOrWhiteSpace(region))
            return Result.Failure<Address>("Region is required.");

        if (string.IsNullOrWhiteSpace(city))
            return Result.Failure<Address>("City is required.");

        if (string.IsNullOrWhiteSpace(street))
            return Result.Failure<Address>("Street is required.");

        if (string.IsNullOrWhiteSpace(house))
            return Result.Failure<Address>("House is required.");

        return Result.Success(new Address(
            postalCode.Trim(),
            region.Trim(),
            city.Trim(),
            street.Trim(),
            house.Trim(),
            apartment?.Trim()));
    }

    public override string ToString()
    {
        return $"{PostalCode}, {Region}, c. {City}, st. {Street}, h. {House}"
               + (string.IsNullOrEmpty(Apartment) ? string.Empty : $", ap. {Apartment}");
    }

}