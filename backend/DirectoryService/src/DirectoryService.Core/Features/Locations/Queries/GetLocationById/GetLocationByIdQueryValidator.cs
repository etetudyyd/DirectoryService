using Core.Validation;
using DirectoryService.Validators;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Queries.GetLocationById;

public class GetLocationByIdQueryValidator : AbstractValidator<GetLocationByIdQuery>
{
    public GetLocationByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired())
            .NotNull().WithError(GeneralErrors.General.ValueIsRequired());
    }
}