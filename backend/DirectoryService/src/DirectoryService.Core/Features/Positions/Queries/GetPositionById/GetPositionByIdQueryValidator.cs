using FluentValidation;

namespace DirectoryService.Features.Positions.Queries.GetPositionById;

public class GetPositionByIdQueryValidator : AbstractValidator<GetPositionByIdQuery>
{
    public GetPositionByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is has to be not empty.")
            .NotNull().WithMessage("Id is has to be not null.");
    }
}