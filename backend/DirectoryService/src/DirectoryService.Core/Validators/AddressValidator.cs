using DirectoryService.Locations;
using FluentValidation;

namespace DirectoryService.Validators;

public class AddressValidator : AbstractValidator<AddressDto>
{
    public AddressValidator()
    {
        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is has to be not empty")
            .Length(Constants.LENGTH_ADDRESS_POSTAL_CODE).WithMessage("Postal code is has to be 6 characters long");
        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Region is has to be not empty");
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is has to be not empty");
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is has to be not empty");
        RuleFor(x => x.House)
            .NotEmpty().WithMessage("House is has to be not empty");
        RuleFor(x => x.Apartment)
            .NotEmpty().WithMessage("Apartment is has to be not empty");
    }
}