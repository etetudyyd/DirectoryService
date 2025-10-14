using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Application.Database.IQueries;
using DirectoryService.Application.Extentions;
using DirectoryService.Contracts.Locations.Requests;
using DirectoryService.Contracts.Locations.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.CQRS.Locations.Queries.GetLocations;

public class GetLocationsHandler : IQueryHandler<GetLocationsResponse, GetLocationsQuery>
{
    private readonly IDapperConnectionFactory _connectionFactory;
    private readonly IValidator<GetLocationsQuery> _validator;
    private readonly ILogger<GetLocationsHandler> _logger;

    public GetLocationsHandler(
        ILogger<GetLocationsHandler> logger,
        IValidator<GetLocationsQuery> validator,
        IDapperConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<GetLocationsResponse, Errors>> Handle(
        GetLocationsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();

        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            conditions.Add("l.name ILIKE @search");
            parameters.Add("search", $"%{query.Search}%");
        }

        parameters.Add("offset", (query.Page - 1) * query.PageSize, DbType.Int32);
        parameters.Add("page_size", query.PageSize, DbType.Int32);

        string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;
        long? totalCount = null;

        var locations = await connection
            .QueryAsync<LocationDto, long, LocationDto>(
                $"""
                 SELECT l.id,
                    l.name,
                    l.timezone,
                    l.is_active,
                    l.created_at,
                    l.update_at,
                    l.apartment,
                    l.city,
                    l.house,
                    l.postal_code,
                    l.region,
                    l.street,
                COUNT(*) OVER() as total_count
                FROM locations l 
                {whereClause}
                ORDER BY l.created_at DESC
                LIMIT @page_size OFFSET @offset;                           
                """,
                splitOn: "total_count",
                map: (locations, count) =>
                {
                    totalCount ??= count;

                    return locations;
                },
                param:parameters);

        _logger.LogInformation("Found {totalCount} locations", totalCount);

        return new GetLocationsResponse(locations.ToList(), totalCount ?? 0);
    }
}

/*public class GetLocationsHandler : IQueryHandler<GetLocationsResponse, GetLocationsQuery>
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
                .Where(l => EF.Functions.ILike(
                    l.Name.Value, $"%{query.Search}%"));
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

}*/