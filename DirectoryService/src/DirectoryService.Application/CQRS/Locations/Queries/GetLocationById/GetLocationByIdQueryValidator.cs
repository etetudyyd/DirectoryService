using FluentValidation;

namespace DirectoryService.Application.CQRS.Locations.Queries.GetLocationById;

public class GetLocationByIdQueryValidator : AbstractValidator<GetLocationByIdQuery>
{
    public GetLocationByIdQueryValidator()
    {

    }
}