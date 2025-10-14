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

namespace DirectoryService.Application.CQRS.Locations.Queries.GetLocations;

public class GetLocationsHandler : IQueryHandler<GetLocationsResponse, GetLocationsQuery>
{
    private readonly IValidator<GetLocationsQuery> _validator;
    private readonly ILogger<GetLocationsHandler> _logger;
    private readonly IReadDbContext _readDbContext;

    public GetLocationsHandler(
        ILogger<GetLocationsHandler> logger,
        IValidator<GetLocationsQuery> validator,
        IReadDbContext readDbContext)
    {
        _logger = logger;
        _validator = validator;
        _readDbContext = readDbContext;
    }

    public async Task<Result<GetLocationsResponse, Errors>> Handle(GetLocationsQuery query, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var locationsQuery = _readDbContext.LocationsRead;

        if (query.Ids is not [])
        {
            // вывести локации департаментов
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            locationsQuery = locationsQuery
                .Where(l => EF.Functions
                    .Like(
                        l.Name.Value.ToLower(),
                        $"%{query.Search.ToLower()}%"));
        }

        // Pagination
        long totalCount = await locationsQuery.LongCountAsync(cancellationToken);

        locationsQuery = locationsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);

        var locations = await locationsQuery
            .OrderBy(l => l.CreatedAt)
            .Select(l => new LocationDto
            {
                Id = l.Id.Value,
                Name = l.Name.Value,
                Address = l.Address.ToString(),
                TimeZone = l.Timezone.Value,
            }).ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} locations", locations.Count);

        return new GetLocationsResponse
        {
            Locations = locations,
            TotalCount = totalCount,
        };
    }

}