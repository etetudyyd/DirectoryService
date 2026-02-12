using DirectoryService.Validators;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Commands.DeactivatePosition;

public class DeactivatePositionCommandValidator : AbstractValidator<DeactivatePositionCommand>
{
    public DeactivatePositionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty()
            .WithError(GeneralErrors.General.ValueIsRequired());
    }
}