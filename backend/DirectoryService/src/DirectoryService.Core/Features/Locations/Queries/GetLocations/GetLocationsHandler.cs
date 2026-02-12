using System.Data;
using Core.Abstractions;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database.ITransactions;
using DirectoryService.Locations;
using DirectoryService.Locations.Responses;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Queries.GetLocations;

public class GetLocationsHandler : IQueryHandler<GetLocationsResponse, GetLocationsQuery>
{
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<GetLocationsHandler> _logger;

    public GetLocationsHandler(
        ILogger<GetLocationsHandler> logger,
        ITransactionManager transactionManager)
    {
        _logger = logger;
        _transactionManager = transactionManager;
    }

    public async Task<Result<GetLocationsResponse, Errors>> Handle(
    GetLocationsQuery query,
    CancellationToken cancellationToken)
{
    var connection = await _transactionManager.GetDbConnectionAsync(cancellationToken);

    var locationsQuery = query.LocationsRequest;
    var parameters = new DynamicParameters();
    var conditions = new List<string>();
    string joins = locationsQuery.DepartmentIds is { Count: > 0 }
        ? $"JOIN {Constants.DEPARTMENT_LOCATIONS_TABLE_ROUTE} dl ON dl.location_id = l.id"
        : string.Empty;

    if (!string.IsNullOrWhiteSpace(locationsQuery.Search))
    {
        conditions.Add("l.name ILIKE @search");
        parameters.Add("search", $"%{locationsQuery.Search}%");
    }

    if (locationsQuery.IsActive.HasValue)
    {
        conditions.Add("l.is_active = @isActive");
        parameters.Add("isActive", locationsQuery.IsActive.Value);

        conditions.Add(locationsQuery.IsActive.Value == false ? "l.deleted_at IS NOT NULL" : "l.deleted_at IS NULL");
    }

    if (locationsQuery.DepartmentIds is { Count: > 0 })
    {
        conditions.Add("dl.department_id = ANY(@departmentIds)");
        parameters.Add("departmentIds", locationsQuery.DepartmentIds);
    }

    parameters.Add("offset", (locationsQuery.Page - 1) * locationsQuery.PageSize, DbType.Int32);
    parameters.Add("page_size", locationsQuery.PageSize, DbType.Int32);

    string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

    int totalItems = 0;

    var items = await connection
        .QueryAsync<LocationDto, AddressDto, int, LocationDto>(
            $"""
             WITH filtered_locations AS (
                 SELECT DISTINCT l.id
                 FROM {Constants.LOCATION_TABLE_ROUTE} l
                 {joins}
                 {whereClause}
             )
             SELECT 
                 l.id,
                 l.name,
                 l.timezone,
                 l.is_active as IsActive,
                 l.created_at as CreatedAt,
                 l.updated_at as UpdatedAt,
                 l.deleted_at as DeletedAt,
                 
                 l.postal_code   AS PostalCode,
                 l.region        AS Region,
                 l.city          AS City,
                 l.street        AS Street,
                 l.house         AS House,
                 l.apartment     AS Apartment,
                 
                 CAST((SELECT COUNT(*) FROM filtered_locations) AS INTEGER) AS total_count
             FROM {Constants.LOCATION_TABLE_ROUTE} l
             WHERE l.id IN (SELECT id FROM filtered_locations)
             ORDER BY l.created_at DESC
             LIMIT @page_size OFFSET @offset;
             """,
            splitOn: "PostalCode, total_count",
            map: (location, address, count) =>
            {
                location.Address = address;
                totalItems = count; // count теперь int
                return location;
            },
            param: parameters);
    
    _logger.LogInformation("Found {totalItems} locations", totalItems);

    return new GetLocationsResponse(
        items.ToList(),
        totalItems,
        locationsQuery.Page!.Value,
        locationsQuery.PageSize!.Value);
}
}