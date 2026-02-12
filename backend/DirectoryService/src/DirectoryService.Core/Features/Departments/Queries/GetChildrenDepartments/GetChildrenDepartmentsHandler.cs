using System.Data;
using Core.Abstractions;
using Core.Caching;
using Core.Validation;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database.ITransactions;
using DirectoryService.Departments.Responses;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Queries.GetChildrenDepartments;

public class GetChildrenDepartmentsHandler : IQueryHandler<PaginationResponse<DepartmentPrefetchResponse>, GetChildrenDepartmentsQuery>
{
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<GetChildrenDepartmentsQuery> _validator;
    private readonly ILogger<GetChildrenDepartmentsHandler> _logger;
    private readonly HybridCache _cache;

    public GetChildrenDepartmentsHandler(
        ITransactionManager transactionManager,
        IValidator<GetChildrenDepartmentsQuery> validator,
        ILogger<GetChildrenDepartmentsHandler> logger,
        HybridCache cache)
    {
        _transactionManager = transactionManager;
        _logger = logger;
        _cache = cache;
        _validator = validator;
    }

    public async Task<Result<PaginationResponse<DepartmentPrefetchResponse>, Errors>> Handle(
        GetChildrenDepartmentsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var queryRequest = query.Request;

        string dapperSql = $"""
                            WITH child_departments AS (
                                SELECT
                                    d.id,
                                    d.name,
                                    d.identifier,
                                    d.path,
                                    d.parent_id AS ParentId,
                                    d.is_active AS IsActive,
                                    d.created_at AS CreatedAt,
                                    d.updated_at AS UpdatedAt
                                FROM {Constants.DEPARTMENT_TABLE_ROUTE} d
                                WHERE d.parent_id = @ParentId
                                ORDER BY d.created_at
                                OFFSET @Offset LIMIT @Limit
                            )
                            SELECT
                                id,
                                name,
                                identifier,
                                path,
                                ParentId,
                                IsActive,
                                CreatedAt,
                                UpdatedAt,
                                EXISTS(
                                    SELECT 1
                                    FROM {Constants.DEPARTMENT_TABLE_ROUTE} d
                                    WHERE d.parent_id = child_departments.id
                                ) AS HasMoreChildren,
                                CAST(COUNT(*) OVER() AS INT) AS total_count
                            FROM child_departments;
                            """;

        var connection = await _transactionManager.GetDbConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("ParentId", query.Request.ParentId);
        parameters.Add("Offset", (query.Request.Page - 1) * query.Request.PageSize, DbType.Int32);
        parameters.Add("Limit", query.Request.PageSize, DbType.Int32);

        int totalItems = 0;

        string prefix = $"{Constants.DEPARTMENT_CACHE_PREFIX}_childs";
        string cacheKey = CacheKeyBuilder.Build(
            prefix,
            ("parent_id", queryRequest.ParentId),
            ("page", queryRequest.Page),
            ("page_size", queryRequest.PageSize));

        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(Constants.TTL_CACHE),
        };

        var items = await _cache.GetOrCreateAsync<IEnumerable<DepartmentPrefetchResponse>>(
            cacheKey,
            async _ => await connection.QueryAsync<
                DepartmentPrefetchResponse,
                int,
                DepartmentPrefetchResponse>(
                dapperSql,
                splitOn: "total_count",
                map: (department, count) =>
                {
                    totalItems = count;
                    return department;
                },
                param: parameters),
            options,
            cancellationToken : cancellationToken);

        _logger.LogInformation("Departments was successfully founded!");

        return new PaginationResponse<DepartmentPrefetchResponse>(
            items.ToList(),
            totalItems,
            queryRequest.Page,
            queryRequest.PageSize);
    }
}