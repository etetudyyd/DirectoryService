using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DevQuestions.Domain;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Application.Database.IQueries;
using DirectoryService.Application.Extentions;
using DirectoryService.Contracts.Departments.Responses;
using DirectoryService.Contracts.Shared;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Features.Departments.Queries.GetChildrenDepartments;

public class GetChildrenDepartmentsHandler : IQueryHandler<GetChildrenDepartmentsResponse, GetChildrenDepartmentsQuery>
{
    private readonly IDapperConnectionFactory _connectionFactory;
    private readonly IValidator<GetChildrenDepartmentsQuery> _validator;
    private readonly ILogger<GetChildrenDepartmentsHandler> _logger;
    private readonly HybridCache _cache;

    public GetChildrenDepartmentsHandler(
        IDapperConnectionFactory connectionFactory,
        IValidator<GetChildrenDepartmentsQuery> validator,
        ILogger<GetChildrenDepartmentsHandler> logger,
        HybridCache cache)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _cache = cache;
        _validator = validator;
    }

    public async Task<Result<GetChildrenDepartmentsResponse, Errors>> Handle(
        GetChildrenDepartmentsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var queryRequest = query.Request;

        string dapperSql = """
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
                               FROM departments d
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
                                   FROM departments d
                                   WHERE d.parent_id = child_departments.id
                               ) AS HasMoreChildren
                           FROM child_departments;
                           """;

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("ParentId", query.Request.ParentId);
        parameters.Add("Offset", (query.Request.Page - 1) * query.Request.PageSize, DbType.Int32);
        parameters.Add("Limit", query.Request.PageSize, DbType.Int32);

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

        var departments = await _cache.GetOrCreateAsync<IEnumerable<DepartmentPrefetchResponse>>(
            cacheKey,
            async _ => await connection.QueryAsync<DepartmentPrefetchResponse>(
                dapperSql,
                param: parameters),
            options,
            cancellationToken : cancellationToken);

        _logger.LogInformation("Departments was successfully founded!");

        return new GetChildrenDepartmentsResponse(departments.ToList());
    }
}