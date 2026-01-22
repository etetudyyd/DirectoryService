using DirectoryService.Features.Locations.Commands.CreateLocation;
using FluentValidation;

namespace DirectoryService.Features.Locations.Commands.UpdateLocation;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(x => x.LocationRequest.Name).SetValidator(new NameValidator());
        RuleFor(x => x.LocationRequest.Address).SetValidator(new AddressValidator());
        RuleFor(x => x.LocationRequest.Timezone).SetValidator(new TimezoneValidator());
    }
}