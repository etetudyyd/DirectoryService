using System.Data;
using Core.Abstractions;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Database.ITransactions;
using DirectoryService.Departments;
using DirectoryService.Departments.Responses;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsHandler : IQueryHandler<PaginationResponse<DepartmentItemDto>, GetDepartmentsQuery>
{
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<GetDepartmentsHandler> _logger;
    private readonly HybridCache _cache;

    public GetDepartmentsHandler(
        ITransactionManager transactionManager,
        ILogger<GetDepartmentsHandler> logger,
        HybridCache cache)
    {
        _logger = logger;
        _cache = cache;
        _transactionManager = transactionManager;
    }

    public async Task<Result<PaginationResponse<DepartmentItemDto>, Errors>> Handle(
        GetDepartmentsQuery query,
        CancellationToken cancellationToken)
    {
        var connection = await _transactionManager.GetDbConnectionAsync(cancellationToken);

        var departmentsQuery = query.DepartmentsRequest;

        var parameters = new DynamicParameters();
        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(departmentsQuery.Search))
        {
            conditions.Add("d.name ILIKE @search");
            parameters.Add("search", $"%{departmentsQuery.Search}%");
        }

        conditions.Add("d.is_active = true");

        parameters.Add("offset", (departmentsQuery.Page - 1) * departmentsQuery.PageSize, DbType.Int32);
        parameters.Add("page_size", departmentsQuery.PageSize, DbType.Int32);

        string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

        int totalItems = 0;

        var items = await connection
            .QueryAsync<DepartmentItemDto, int, DepartmentItemDto>(
                $"""
                 SELECT 
                     d.id,
                     d.name,
                     
                     CAST(COUNT(*) OVER() AS INT) AS total_count
                 FROM {Constants.DEPARTMENT_TABLE_ROUTE} d
                 {whereClause}
                 ORDER BY d.created_at DESC
                 LIMIT @page_size OFFSET @offset;
                 """,
                splitOn: "total_count",
                map: (department, count) =>
                {
                    totalItems = count;
                    return department;
                },
                param: parameters);

        _logger.LogInformation("Departments was successfully founded!");

        return new PaginationResponse<DepartmentItemDto>(
            items.ToList(),
            totalItems,
            departmentsQuery.Page!.Value,
            departmentsQuery.PageSize!.Value);
    }
}