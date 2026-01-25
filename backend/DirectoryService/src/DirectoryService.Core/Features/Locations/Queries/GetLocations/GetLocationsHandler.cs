using System.Data;
using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database.IQueries;
using DirectoryService.Locations;
using DirectoryService.Locations.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Queries.GetLocations;

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
        var joins = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            conditions.Add("l.name ILIKE @search");
            parameters.Add("search", $"%{query.Search}%");
        }

        if (query.IsActive.HasValue)
        {
            conditions.Add("l.is_active = @isActive");
            parameters.Add("isActive", query.IsActive.Value);
        }


        if (query.Ids is { Count: > 0 })
        {
            joins.Add("JOIN department_locations dl ON dl.location_id = l.id");
            conditions.Add("dl.department_id = ANY(@departmentIds)");
            parameters.Add("departmentIds", query.Ids);
        }

        parameters.Add("offset", (query.Page - 1) * query.PageSize, DbType.Int32);
        parameters.Add("page_size", query.PageSize, DbType.Int32);

        string joinClause = joins.Count > 0 ? string.Join(" ", joins) : string.Empty;
        string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

        int totalItems = 0;

        var items = await connection
            .QueryAsync<LocationDto, AddressDto, int, LocationDto>(
                $"""
                 SELECT 
                     l.id,
                     l.name,
                     l.timezone,
                     l.is_active as isActive,
                     l.created_at,
                     l.updated_at,
                     
                     l.postal_code   AS PostalCode,
                     l.region        AS Region,
                     l.city          AS City,
                     l.street        AS Street,
                     l.house         AS House,
                     l.apartment     AS Apartment,
                     
                     CAST(COUNT(*) OVER() AS INT) AS total_count
                 FROM locations l
                 {joinClause}
                 {whereClause}
                 ORDER BY l.created_at DESC
                 LIMIT @page_size OFFSET @offset;
                 """,
                splitOn: "PostalCode, total_count",
                map: (location, address, count) =>
                {
                    location.Address = address;

                    totalItems = count;
                    return location;
                },
                param: parameters);

        _logger.LogInformation("Found {totalItems} locations", totalItems);

        return new GetLocationsResponse(
            items.ToList(),
            totalItems,
            query.Page,
            query.PageSize);
    }
}