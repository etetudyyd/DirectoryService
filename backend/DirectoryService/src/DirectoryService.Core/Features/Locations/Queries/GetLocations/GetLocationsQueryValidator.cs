using FluentValidation;

namespace DirectoryService.Features.Locations.Queries.GetLocations;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
        RuleFor(x => x.Search)
            .NotEmpty().WithMessage("Search is has to be not empty.")
            .NotNull().WithMessage("Search is has to be null.");
        RuleForEach(x => x.Ids)
            .NotEmpty().WithMessage("Request is has to contains at least one Id.")
            .NotNull().WithMessage("Id is has to be null.");

    }
}