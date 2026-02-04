using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IQueries;
using DirectoryService.Positions;
using DirectoryService.Positions.Responses;
using DirectoryService.ValueObjects.Position;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Queries.GetPositionById;

public class GetPositionByIdHandler : IQueryHandler<GetPositionByIdResponse, GetPositionByIdQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IValidator<GetPositionByIdQuery> _validator;
    private readonly ILogger<GetPositionByIdHandler> _logger;

    public GetPositionByIdHandler(IReadDbContext readDbContext, IValidator<GetPositionByIdQuery> validator, ILogger<GetPositionByIdHandler> logger)
    {
        _readDbContext = readDbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetPositionByIdResponse, Errors>> Handle(
        GetPositionByIdQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var position = await _readDbContext.PositionsRead
            .FirstOrDefaultAsync(l => l.Id == new PositionId(query.Id), cancellationToken);

        if (position is null)
        {
            return Error.NotFound("location.not.found", "Location not found")
                .ToErrors();
        }

        _logger.LogInformation("Get location by id: {Id}", position.Id);

        return new GetPositionByIdResponse(
            new PositionDto
            {
                Id = position.Id.Value,
                Name = position.Name.Value,
                Description = position.Description.Value,
                IsActive = position.IsActive,
                CreatedAt = position.CreatedAt,
                UpdatedAt = position.UpdatedAt,
                DeletedAt = position.DeletedAt,
            });
    }
}