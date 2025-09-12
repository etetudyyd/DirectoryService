using DirectoryService.Contracts.Locations;
using FluentValidation;

namespace DirectoryService.Application.Locations;

public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationValidator()
    {
        //RuleFor(x => x.Name).NotEmpty().NotNull().MaximumLength(100);
    }
}