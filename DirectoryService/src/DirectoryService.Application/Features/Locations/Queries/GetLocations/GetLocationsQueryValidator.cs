using FluentValidation;

namespace DirectoryService.Application.Features.Locations.Queries.GetLocations;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator() { }
}