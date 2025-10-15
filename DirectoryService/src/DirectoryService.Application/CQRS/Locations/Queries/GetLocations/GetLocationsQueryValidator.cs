using FluentValidation;

namespace DirectoryService.Application.CQRS.Locations.Queries.GetLocations;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator() { }
}