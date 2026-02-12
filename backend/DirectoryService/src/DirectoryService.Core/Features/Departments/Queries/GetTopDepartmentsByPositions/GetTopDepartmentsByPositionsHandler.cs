using Core.Abstractions;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database.ITransactions;
using DirectoryService.Departments;
using DirectoryService.Departments.Responses;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Queries.GetTopDepartmentsByPositions;

public class GetTopDepartmentsByPositionsHandler : IQueryHandler<GetTopDepartmentsByPositionsResponse, GetTopDepartmentsByPositionsQuery>
{
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<GetTopDepartmentsByPositionsHandler> _logger;
    private readonly HybridCache _cache;

    public GetTopDepartmentsByPositionsHandler(
        ITransactionManager transactionManager,
        ILogger<GetTopDepartmentsByPositionsHandler> logger,
        HybridCache cache)
    {
        _transactionManager = transactionManager;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<GetTopDepartmentsByPositionsResponse, Errors>> Handle(
        GetTopDepartmentsByPositionsQuery query,
        CancellationToken cancellationToken)
    {
        var connection = await _transactionManager.GetDbConnectionAsync(cancellationToken);

        string cacheKey = $"{Constants.DEPARTMENT_CACHE_PREFIX}_childs_top5";

        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(Constants.TTL_CACHE),
        };

        var departments = await _cache.GetOrCreateAsync<IEnumerable<DepartmentDto>>(
            cacheKey,
            async _ => await connection
                .QueryAsync<DepartmentDto>($"""
                                            SELECT
                                                d.id AS "Id",
                                                d.name AS "Name",
                                                d.identifier AS "Identifier",
                                                d.path AS "Path",
                                                d.parent_id AS "ParentId",
                                                d.is_active AS "IsActive",
                                                d.created_at AS "CreatedAt",
                                                d.updated_at AS "UpdatedAt",
                                                COUNT(dp.position_id) AS "PositionCount"
                                            FROM {Constants.SCHEMA}.departments d
                                            LEFT JOIN {Constants.SCHEMA}.department_positions dp ON dp.department_id = d.id
                                            GROUP BY d.id, d.name, d.identifier, d.path, d.parent_id, d.is_active, d.created_at, d.updated_at
                                            ORDER BY "PositionCount" DESC
                                            LIMIT 5;
                                            """),
            options,
            cancellationToken : cancellationToken);

        _logger.LogInformation("Departments was successfully founded!");

        return new GetTopDepartmentsByPositionsResponse(departments.ToList());
    }
}