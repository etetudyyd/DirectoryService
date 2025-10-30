using DevQuestions.Domain;
using DirectoryService.Contracts.Locations.Dtos;
using FluentValidation;

namespace DirectoryService.Application.Features.Locations.Commands.CreateLocation;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(x => x.LocationRequest.Name).SetValidator(new NameValidator());
        RuleFor(x => x.LocationRequest.Address).SetValidator(new AddressValidator());
        RuleFor(x => x.LocationRequest.Timezone).SetValidator(new TimezoneValidator());
        RuleForEach(x => x.LocationRequest.DepartmentLocations).NotEmpty().WithMessage("Department locations should not be empty");
    }
}

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

public class NameValidator : AbstractValidator<NameDto>
{
    public NameValidator()
    {
        RuleFor(x => x.Value)
            .MinimumLength(Constants.MIN_LENGTH_LOCATION_NAME).WithMessage("Name is has to be at least 3 characters long")
            .MaximumLength(Constants.MAX_LENGTH_LOCATION_NAME).WithMessage("Name is has to be at most 100 characters long")
            .NotEmpty().WithMessage("Name is has to be not empty");
    }
}

public class TimezoneValidator : AbstractValidator<TimezoneDto>
{
    public TimezoneValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Timezone is has to be not empty");
    }
}