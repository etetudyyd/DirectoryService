using System.Data;
using Core.Abstractions;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database.ITransactions;
using DirectoryService.Locations;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Queries.GetLocationDictionary;

public class GetLocationDictionaryHandler : IQueryHandler<PaginationResponse<LocationItemDto>, GetLocationDictionaryQuery>
{
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<GetLocationDictionaryHandler> _logger;
    private readonly HybridCache _cache;

    public GetLocationDictionaryHandler(
        ITransactionManager transactionManager,
        ILogger<GetLocationDictionaryHandler> logger,
        HybridCache cache)
    {
        _transactionManager = transactionManager;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<PaginationResponse<LocationItemDto>, Errors>> Handle(
        GetLocationDictionaryQuery query,
        CancellationToken cancellationToken)
    {
        var connection = await _transactionManager.GetDbConnectionAsync(cancellationToken);

        var locationsQuery = query.Request;

        var parameters = new DynamicParameters();
        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(locationsQuery.Search))
        {
            conditions.Add("d.name ILIKE @search");
            parameters.Add("search", $"%{locationsQuery.Search}%");
        }

        conditions.Add("d.is_active = true");

        parameters.Add("offset", (locationsQuery.Page - 1) * locationsQuery.PageSize, DbType.Int32);
        parameters.Add("page_size", locationsQuery.PageSize, DbType.Int32);

        string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

        int totalItems = 0;

        var items = await connection
            .QueryAsync<LocationItemDto, int, LocationItemDto>(
                $"""
                 SELECT 
                     d.id,
                     d.name,
                     
                     CAST(COUNT(*) OVER() AS INT) AS total_count
                 FROM {Constants.LOCATION_TABLE_ROUTE} d
                 {whereClause}
                 ORDER BY d.created_at DESC
                 LIMIT @page_size OFFSET @offset;
                 """,
                splitOn: "total_count",
                map: (location, count) =>
                {
                    totalItems = count;
                    return location;
                },
                param: parameters);

        _logger.LogInformation("Locations was successfully founded!");

        return new PaginationResponse<LocationItemDto>(
            items.ToList(),
            totalItems,
            locationsQuery.Page!.Value,
            locationsQuery.PageSize!.Value);
    }
    }