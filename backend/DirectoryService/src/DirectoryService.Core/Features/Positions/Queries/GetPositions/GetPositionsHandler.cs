using System.Data;
using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database.IQueries;
using DirectoryService.Positions;
using DirectoryService.Positions.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Queries.GetPositions;

public class GetPositionsHandler : IQueryHandler<GetPositionsResponse, GetPositionsQuery>
{
    private readonly IDapperConnectionFactory _connectionFactory;
    private readonly IValidator<GetPositionsQuery> _validator;
    private readonly ILogger<GetPositionsHandler> _logger;

    public GetPositionsHandler(
        IDapperConnectionFactory connectionFactory,
        IValidator<GetPositionsQuery> validator,
        ILogger<GetPositionsHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetPositionsResponse, Errors>> Handle(
        GetPositionsQuery query,
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
            conditions.Add("p.name ILIKE @search");
            parameters.Add("search", $"%{query.Search}%");
        }

        if (query.IsActive.HasValue)
        {
            conditions.Add("p.is_active = @isActive");
            parameters.Add("isActive", query.IsActive.Value);

            conditions.Add(query.IsActive.Value == false ? "p.deleted_at IS NOT NULL" : "p.deleted_at IS NULL");
        }

        if (query.DepartmentsIds is { Count: > 0 })
        {
            joins.Add("JOIN department_positions dp ON dp.position_id = p.id");
            conditions.Add("dp.department_id = ANY(@departmentIds)");
            parameters.Add("departmentIds", query.DepartmentsIds);
        }

        parameters.Add("offset", (query.Page - 1) * query.PageSize, DbType.Int32);
        parameters.Add("page_size", query.PageSize, DbType.Int32);

        string joinClause = joins.Count > 0 ? string.Join(" ", joins) : string.Empty;
        string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

        int totalItems = 0;

        var items = await connection
            .QueryAsync<PositionDto, int, int, PositionDto>(
                $"""
                 SELECT 
                     p.id,
                     p.name,
                     p.description,
                     p.is_active as isActive,
                     p.created_at as CreatedAt,
                     p.updated_at as UpdatedAt,
                     p.deleted_at as DeletedAt,
                     
                     -- Количество департаментов
                     COUNT(DISTINCT dp.department_id)::INTEGER as departmentCount,

                     CAST(COUNT(*) OVER() AS INT) AS total_count
                 FROM positions p

                 -- LEFT JOIN чтобы получить все позиции, даже без департаментов
                 LEFT JOIN department_positions dp ON dp.position_id = p.id

                 {joinClause}
                 {whereClause}

                 GROUP BY p.id, p.name, p.description, p.is_active, p.created_at, p.updated_at, p.deleted_at
                 ORDER BY p.created_at DESC
                 LIMIT @page_size OFFSET @offset;
                 """,
                splitOn: "departmentCount,total_count",
                map: (position, departmentCount, totalCount) =>
                {
                    position.DepartmentCount = departmentCount; // Добавьте это поле в DTO
                    totalItems = totalCount;
                    return position;
                },
                param: parameters);

        _logger.LogInformation("Found {totalItems} positions", totalItems);

        return new GetPositionsResponse(
            items.ToList(),
            totalItems,
            query.Page,
            query.PageSize);
    }
}