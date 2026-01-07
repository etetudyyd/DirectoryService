using FluentValidation;

namespace DirectoryService.Features.Locations.Queries.GetLocations;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    { }
}