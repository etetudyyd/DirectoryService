using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Application.Database.IQueries;
using DirectoryService.Application.Extentions;
using DirectoryService.Contracts.Locations.Dtos;
using DirectoryService.Contracts.Locations.Requests;
using DirectoryService.Contracts.Locations.Responses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.CQRS.Locations.Queries.GetLocationById;

public class GetLocationByIdHandler : IQueryHandler<GetLocationByIdResponse, GetLocationByIdQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IValidator<GetLocationByIdQuery> _validator;
    private readonly ILogger<GetLocationByIdHandler> _logger;

    public GetLocationByIdHandler(
        IReadDbContext readDbContext,
        ILogger<GetLocationByIdHandler> logger,
        IValidator<GetLocationByIdQuery> validator)
    {
        _readDbContext = readDbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetLocationByIdResponse, Errors>> Handle(GetLocationByIdQuery query, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var location = await _readDbContext.LocationsRead
            .FirstOrDefaultAsync(l => l.Id == new LocationId(query.Id), cancellationToken);

        if (location is null)
        {
            return Error.NotFound("location.not.found", "Location not found")
                .ToErrors();
        }

        _logger.LogInformation("Get location by id: {Id}", location.Id);

        return new GetLocationByIdResponse(
            new LocationDto
        {
            Id = location.Id.Value,
            Name = location.Name.Value,
            Address = location.Address.ToString(),
            TimeZone = location.Timezone.Value,
        });
    }
}