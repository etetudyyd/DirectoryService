using FluentValidation;

namespace DirectoryService.Features.Positions.Commands.DeactivatePosition;

public class DeactivatePositionCommandValidator : AbstractValidator<DeactivatePositionCommand>
{
    public DeactivatePositionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Position id is has to be not empty");
    }
}