using Core.Validation;
using DirectoryService.Validators;
using DirectoryService.ValueObjects.Position;
using FluentValidation;

namespace DirectoryService.Features.Positions.Commands.UpdatePosition;

public class UpdatePositionCommandValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionCommandValidator()
    {
        RuleFor(x => x.PositionRequest.Name)
            .MustBeValueObject(PositionName.Create);
        RuleFor(x => x.PositionRequest.Description)
            .MustBeValueObject(PositionDescription.Create);
    }
}