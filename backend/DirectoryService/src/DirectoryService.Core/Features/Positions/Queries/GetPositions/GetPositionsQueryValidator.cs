using FluentValidation;

namespace DirectoryService.Features.Positions.Queries.GetPositions;

public class GetPositionsQueryValidator : AbstractValidator<GetPositionsQuery>
{
    public GetPositionsQueryValidator()
    { }
}