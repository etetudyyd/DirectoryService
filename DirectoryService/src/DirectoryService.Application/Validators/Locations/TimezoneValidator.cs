using DirectoryService.Contracts.Locations;
using FluentValidation;

namespace DirectoryService.Application.Validators.Locations;

public class TimezoneValidator : AbstractValidator<TimezoneDto>
{
    public TimezoneValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Timezone is has to be not empty");
    }
}