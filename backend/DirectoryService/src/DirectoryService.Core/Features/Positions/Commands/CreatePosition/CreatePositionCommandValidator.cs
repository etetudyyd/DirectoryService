using Core.Validation;
using DirectoryService.Validators;
using DirectoryService.ValueObjects.Position;
using FluentValidation;

namespace DirectoryService.Features.Positions.Commands.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(x => x.PositionRequest.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(x => x.PositionRequest.Description)
            .MustBeValueObject(PositionDescription.Create);
    }
}