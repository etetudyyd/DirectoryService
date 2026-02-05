using FluentValidation;

namespace DirectoryService.Features.Positions.Commands.UpdatePosition;

public class UpdatePositionCommandValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionCommandValidator()
    {
        RuleFor(x => x.PositionRequest.Name)
            .MinimumLength(Constants.MIN_LENGTH_LOCATION_NAME).WithMessage($"Name is has to be at least {Constants.MIN_LENGTH_LOCATION_NAME} characters long")
            .MaximumLength(Constants.MAX_LENGTH_LOCATION_NAME).WithMessage($"Name is has to be at most {Constants.MAX_LENGTH_LOCATION_NAME} characters long")
            .NotEmpty().WithMessage("Name is has to be not empty");
        RuleFor(x => x.PositionRequest.Description)
            .MaximumLength(Constants.MAX_LENGTH_POSITION_DESCRIPTION).WithMessage($"Description is has to be at most {Constants.MAX_LENGTH_POSITION_DESCRIPTION} characters long");
    }

}