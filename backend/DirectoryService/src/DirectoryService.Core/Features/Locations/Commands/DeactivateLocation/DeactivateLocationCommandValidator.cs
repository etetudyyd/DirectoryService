using DirectoryService.Features.Locations.Commands.CreateLocation;
using FluentValidation;

namespace DirectoryService.Features.Locations.Commands.DeactivateLocation;

public class DeactivateLocationCommandValidator : AbstractValidator<DeactivateLocationCommand>
{
    public DeactivateLocationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Location id is has to be not empty");
    }
}


