using DirectoryService.Locations;
using FluentValidation;

namespace DirectoryService.Features.Locations.Commands.CreateLocation;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(x => x.LocationRequest.Name)
            .MinimumLength(Constants.MIN_LENGTH_LOCATION_NAME).WithMessage("Name is has to be at least 3 characters long")
            .MaximumLength(Constants.MAX_LENGTH_LOCATION_NAME).WithMessage("Name is has to be at most 100 characters long")
            .NotEmpty().WithMessage("Name is has to be not empty");
        RuleFor(x => x.LocationRequest.Address).SetValidator(new AddressValidator());
        RuleFor(x => x.LocationRequest.Timezone)
            .NotEmpty().WithMessage("Timezone is has to be not empty");
        RuleForEach(x => x.LocationRequest.DepartmentsIds).NotEmpty().WithMessage("Department locations should not be empty");
    }
}

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
