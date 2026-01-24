using DirectoryService.Features.Locations.Commands.CreateLocation;
using FluentValidation;

namespace DirectoryService.Features.Locations.Commands.UpdateLocation;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
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