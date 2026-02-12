using System.Data;
using Core.Abstractions;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database.ITransactions;
using DirectoryService.Positions;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Queries.GetPositions;

public class GetPositionsHandler : IQueryHandler<PaginationResponse<PositionDto>, GetPositionsQuery>
{
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<GetPositionsHandler> _logger;

    public GetPositionsHandler(
        ILogger<GetPositionsHandler> logger,
        ITransactionManager transactionManager)
    {
        _logger = logger;
        _transactionManager = transactionManager;
    }

    public async Task<Result<PaginationResponse<PositionDto>, Errors>> Handle(
        GetPositionsQuery query,
        CancellationToken cancellationToken)
    {
        var connection = await _transactionManager.GetDbConnectionAsync(cancellationToken);

        var positionQuery = query.PositionsRequest;

        var parameters = new DynamicParameters();
        var conditions = new List<string>();
        string joins = positionQuery.DepartmentsIds is { Count: > 0 }
            ? $"JOIN {Constants.DEPARTMENT_POSITIONS_TABLE_ROUTE} dp ON dp.position_id = p.id"
            : string.Empty;

        if (!string.IsNullOrWhiteSpace(positionQuery.Search))
        {
            conditions.Add("p.name ILIKE @search");
            parameters.Add("search", $"%{positionQuery.Search}%");
        }

        if (positionQuery.IsActive.HasValue)
        {
            conditions.Add("p.is_active = @isActive");
            parameters.Add("isActive", positionQuery.IsActive.Value);

            conditions.Add(positionQuery.IsActive.Value == false ? "p.deleted_at IS NOT NULL" : "p.deleted_at IS NULL");
        }

        if (positionQuery.DepartmentsIds is { Count: > 0 })
        {
            conditions.Add("dp.department_id = ANY(@departmentIds)");
            parameters.Add("departmentIds", positionQuery.DepartmentsIds);
        }

        parameters.Add("offset", (positionQuery.Page - 1) * positionQuery.PageSize, DbType.Int32);
        parameters.Add("page_size", positionQuery.PageSize, DbType.Int32);

        string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

        int totalItems = 0;

        var items = await connection
            .QueryAsync<PositionDto, int, int, PositionDto>(
                $"""
                 SELECT 
                     p.id,
                     p.name,
                     p.description,
                     p.is_active as IsActive,
                     p.created_at as CreatedAt,
                     p.updated_at as UpdatedAt,
                     p.deleted_at as DeletedAt,
                     
                     COALESCE((
                         SELECT COUNT(DISTINCT department_id)
                         FROM {Constants.DEPARTMENT_POSITIONS_TABLE_ROUTE} dp
                         WHERE dp.position_id = p.id
                     ), 0)::INTEGER as departmentCount,

                     CAST(COUNT(*) OVER() AS INT) AS total_count
                 FROM {Constants.POSITION_TABLE_ROUTE} p
                 {joins}
                 {whereClause}
                 GROUP BY p.id, p.name, p.description, p.is_active, p.created_at, p.updated_at, p.deleted_at
                 ORDER BY p.created_at DESC
                 LIMIT @page_size OFFSET @offset;
                 """,
                splitOn: "departmentCount,total_count",
                map: (position, departmentCount, totalCount) =>
                {
                    position.DepartmentCount = departmentCount;
                    totalItems = totalCount;
                    return position;
                },
                param: parameters);

        _logger.LogInformation("Found {totalItems} positions", totalItems);

        return new PaginationResponse<PositionDto>(
            items.ToList(),
            totalItems,
            positionQuery.Page!.Value,
            positionQuery.PageSize!.Value);
    }
}