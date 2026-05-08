using System.Data;
using Core.Abstractions;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database.ITransactions;
using DirectoryService.Departments;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsHandler : IQueryHandler<PaginationResponse<DepartmentItemDto>, GetDepartmentsQuery>
{
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<GetDepartmentsHandler> _logger;

    public GetDepartmentsHandler(
        ITransactionManager transactionManager,
        ILogger<GetDepartmentsHandler> logger)
    {
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<PaginationResponse<DepartmentItemDto>, Errors>> Handle(
    GetDepartmentsQuery query,
    CancellationToken cancellationToken)
{
    var connection = await _transactionManager.GetDbConnectionAsync(cancellationToken);

    var request = query.Request;

    var parameters = new DynamicParameters();
    var conditions = new List<string>();
    var joins = new List<string>();

    if (request.LocationsIds is { Count: > 0 })
    {
        joins.Add($"""
            JOIN {Constants.DEPARTMENT_LOCATIONS_TABLE_ROUTE} dl
                ON dl.department_id = d.id
        """);
    }

    if (!string.IsNullOrWhiteSpace(request.Search))
    {
        conditions.Add("""
            (
                d.name ILIKE @search
                OR d.identifier ILIKE @search
            )
        """);

        parameters.Add("search", $"%{request.Search}%");
    }

    if (request.ParentId.HasValue)
    {
        conditions.Add("d.parent_id = @parentId");

        parameters.Add("parentId", request.ParentId.Value);
    }

    if (request.LocationsIds is { Count: > 0 })
    {
        conditions.Add("dl.location_id = ANY(@locationIds)");

        parameters.Add("locationIds", request.LocationsIds);
    }

    if (request.IsActive.HasValue)
    {
        conditions.Add("d.is_active = @isActive");

        parameters.Add("isActive", request.IsActive.Value);
    }

    // SORTING
    string orderBy = request.SortBy?.ToLower() switch
    {
        "name" => "d.name",
        "path" => "d.path",
        _ => "d.created_at",
    };

    string sortDirection = request.SortDirection?.ToLower() == "asc"
        ? "ASC"
        : "DESC";

    parameters.Add("offset", (request.Page - 1) * request.PageSize, DbType.Int32);
    parameters.Add("page_size", request.PageSize, DbType.Int32);

    string whereClause = conditions.Count > 0
        ? "WHERE " + string.Join(" AND ", conditions)
        : string.Empty;

    string joinsClause = string.Join("\n", joins);

    int totalItems = 0;

    var items = await connection.QueryAsync<DepartmentItemDto, int, int, DepartmentItemDto>(
        $"""
        SELECT
            d.id,
            d.name,
            d.identifier,
            d.path,
            d.is_active as IsActive,
            d.created_at as CreatedAt,
            d.updated_at as UpdatedAt,
            d.deleted_at as DeletedAt,

            COALESCE((
                SELECT COUNT(DISTINCT dl.location_id)
                FROM {Constants.DEPARTMENT_LOCATIONS_TABLE_ROUTE} dl
                WHERE dl.department_id = d.id
            ), 0)::INTEGER as locationCount,

            CAST(COUNT(*) OVER() AS INT) AS total_count

        FROM {Constants.DEPARTMENT_TABLE_ROUTE} d
        {joinsClause}
        {whereClause}

        GROUP BY
            d.id,
            d.name,
            d.identifier,
            d.path,
            d.is_active,
            d.created_at,
            d.updated_at,
            d.deleted_at

        ORDER BY {orderBy} {sortDirection}

        LIMIT @page_size
        OFFSET @offset;
        """,
        splitOn: "locationCount,total_count",
        map: (department, locationCount, totalCount) =>
        {
            department.LocationCount = locationCount;
            totalItems = totalCount;

            return department;
        },
        param: parameters);

    _logger.LogInformation("Found {totalItems} departments", totalItems);

    return new PaginationResponse<DepartmentItemDto>(
        items.ToList(),
        totalItems,
        request.Page!.Value,
        request.PageSize!.Value);
}
}