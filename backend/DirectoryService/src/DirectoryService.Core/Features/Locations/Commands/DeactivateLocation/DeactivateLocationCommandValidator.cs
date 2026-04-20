using Core.Validation;
using DirectoryService.Features.Locations.Commands.CreateLocation;
using DirectoryService.Validators;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Commands.DeactivateLocation;

public class DeactivateLocationCommandValidator : AbstractValidator<DeactivateLocationCommand>
{
    public DeactivateLocationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithError(GeneralErrors.General.ValueIsRequired());
    }
}


