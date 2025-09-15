using DevQuestions.Domain;
using DirectoryService.Contracts.Locations;
using FluentValidation;

namespace DirectoryService.Application.Validators.Locations;

public class NameValidator : AbstractValidator<NameDto>
{
    public NameValidator()
    {
        RuleFor(x => x.Value)
            .MinimumLength(LengthConstants.MIN_LENGTH_LOCATION_NAME)
            .MaximumLength(LengthConstants.MAX_LENGTH_LOCATION_NAME)
            .NotEmpty()
            .WithMessage("Name is has to be not empty");
    }
}