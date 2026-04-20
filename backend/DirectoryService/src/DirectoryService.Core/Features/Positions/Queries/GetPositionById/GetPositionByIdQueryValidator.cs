using Core.Validation;
using DirectoryService.Validators;
using FluentValidation;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Queries.GetPositionById;

public class GetPositionByIdQueryValidator : AbstractValidator<GetPositionByIdQuery>
{
    public GetPositionByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithError(GeneralErrors.General.ValueIsRequired())
            .NotNull().WithError(GeneralErrors.General.ValueIsRequired());
    }
}