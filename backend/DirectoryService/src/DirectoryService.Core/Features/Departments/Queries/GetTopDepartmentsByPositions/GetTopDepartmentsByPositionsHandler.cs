using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database.IQueries;
using DirectoryService.Departments.Responses;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Queries.GetTopDepartmentsByPositions;

public class GetTopDepartmentsByPositionsHandler : IQueryHandler<GetTopDepartmentsByPositionsResponse, GetTopDepartmentsByPositionsQuery>
{
    private readonly IDapperConnectionFactory _connectionFactory;
    private readonly IValidator<GetTopDepartmentsByPositionsQuery> _validator;
    private readonly ILogger<GetTopDepartmentsByPositionsHandler> _logger;
    private readonly HybridCache _cache;


    public GetTopDepartmentsByPositionsHandler(
        IDapperConnectionFactory connectionFactory,
        ILogger<GetTopDepartmentsByPositionsHandler> logger,
        IValidator<GetTopDepartmentsByPositionsQuery> validator, HybridCache cache)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _validator = validator;
        _cache = cache;
    }

    public async Task<Result<GetTopDepartmentsByPositionsResponse, Errors>> Handle(
        GetTopDepartmentsByPositionsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        string dapperSql = """
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
                           FROM departments d
                           LEFT JOIN department_positions dp ON dp.department_id = d.id
                           GROUP BY d.id, d.name, d.identifier, d.path, d.parent_id, d.is_active, d.created_at, d.updated_at
                           ORDER BY "PositionCount" DESC
                           LIMIT 5;
                           """;

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        string cacheKey = $"{Constants.DEPARTMENT_CACHE_PREFIX}_childs_top5";

        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(Constants.TTL_CACHE),
        };

        var departments = await _cache.GetOrCreateAsync<IEnumerable<DepartmentResponse>>(
            cacheKey,
            async _ => await connection
                .QueryAsync<DepartmentResponse>(dapperSql),
            options,
            cancellationToken : cancellationToken);


        _logger.LogInformation("Departments was successfully founded!");

        return new GetTopDepartmentsByPositionsResponse(departments.ToList());
    }
}