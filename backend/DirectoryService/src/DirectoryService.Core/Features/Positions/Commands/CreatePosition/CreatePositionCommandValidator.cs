using FluentValidation;

namespace DirectoryService.Features.Positions.Commands.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(x => x.PositionRequest.Name)
            .MinimumLength(Constants.MIN_LENGTH_POSITION_NAME).WithMessage($"Name is has to be at least {Constants.MIN_LENGTH_POSITION_NAME} characters long")
            .MaximumLength(Constants.MAX_LENGTH_POSITION_NAME).WithMessage($"Name is has to be at most {Constants.MAX_LENGTH_POSITION_NAME} characters long")
            .NotEmpty().WithMessage("Name is has to be not empty");

        RuleFor(x => x.PositionRequest.Description)
            .MinimumLength(Constants.MIN_LENGTH_POSITION_NAME).WithMessage($"Description is has to be at least {Constants.MIN_LENGTH_POSITION_NAME} characters long")
            .MaximumLength(Constants.MAX_LENGTH_POSITION_NAME).WithMessage($"Description is has to be at most {Constants.MAX_LENGTH_POSITION_NAME} characters long")
            .NotEmpty().WithMessage("Description is has to be not empty");
    }
}