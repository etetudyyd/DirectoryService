using DirectoryService.Contracts.Locations;
using FluentValidation;

namespace DirectoryService.Application.Validators.Locations;

public class AddressValidator : AbstractValidator<AddressDto>
{
    public AddressValidator()
    {
        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is has to be not empty");
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