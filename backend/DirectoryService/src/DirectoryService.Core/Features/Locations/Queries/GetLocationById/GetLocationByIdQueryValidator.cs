using FluentValidation;

namespace DirectoryService.Features.Locations.Queries.GetLocationById;

public class GetLocationByIdQueryValidator : AbstractValidator<GetLocationByIdQuery>
{
    public GetLocationByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is has to be not empty.")
            .NotNull().WithMessage("Id is has to be not empty.");
    }
}