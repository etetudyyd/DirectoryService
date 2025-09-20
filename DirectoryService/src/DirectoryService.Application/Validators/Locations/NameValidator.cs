using DevQuestions.Domain;
using DirectoryService.Contracts.Locations;
using FluentValidation;

namespace DirectoryService.Application.Validators.Locations;

public class NameValidator : AbstractValidator<NameDto>
{
    public NameValidator()
    {
        RuleFor(x => x.Value)
            .MinimumLength(LengthConstants.MIN_LENGTH_LOCATION_NAME).WithMessage("Name is has to be at least 3 characters long")
            .MaximumLength(LengthConstants.MAX_LENGTH_LOCATION_NAME).WithMessage("Name is has to be at most 100 characters long")
            .NotEmpty().WithMessage("Name is has to be not empty");
    }
}