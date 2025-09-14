using DirectoryService.Contracts.Locations;
using FluentValidation;
using FluentValidation.Validators;

namespace DirectoryService.Application.Validators.Locations;

public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Name).SetValidator(new NameValidator());
        RuleFor(x => x.Address).SetValidator(new AddressValidator());
        RuleFor(x => x.Timezone).SetValidator(new TimezoneValidator());
        RuleForEach(x => x.DepartmentLocations).NotEmpty().WithMessage("Department locations should not be empty");
    }
}