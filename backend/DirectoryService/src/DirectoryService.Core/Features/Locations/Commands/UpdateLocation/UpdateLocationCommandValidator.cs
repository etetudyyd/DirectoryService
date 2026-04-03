using Core.Validation;
using DirectoryService.Features.Locations.Commands.CreateLocation;
using DirectoryService.Validators;
using DirectoryService.ValueObjects.Location;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Commands.UpdateLocation;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(x => x.LocationRequest.Name)
            .MustBeValueObject(LocationName.Create);
        RuleFor(x => x.LocationRequest.Address).SetValidator(new AddressValidator());
        RuleFor(x => x.LocationRequest.Timezone)
            .MustBeValueObject(Timezone.Create);
        RuleForEach(x => x.LocationRequest.DepartmentsIds)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired());
    }
}