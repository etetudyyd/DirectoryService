using System.Data;
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

namespace DirectoryService.Features.Departments.Queries.GetDepartmentsDictionary;

public class GetDepartmentsDictionaryHandler : IQueryHandler<GetDepartmentsDictionaryResponse, GetDepartmentsDictionaryQuery>
{
    private readonly IDapperConnectionFactory _connectionFactory;
    private readonly IValidator<GetDepartmentsDictionaryQuery> _validator;
    private readonly ILogger<GetDepartmentsDictionaryHandler> _logger;
    private readonly HybridCache _cache;

    public GetDepartmentsDictionaryHandler(
        IDapperConnectionFactory connectionFactory,
        IValidator<GetDepartmentsDictionaryQuery> validator,
        ILogger<GetDepartmentsDictionaryHandler> logger,
        HybridCache cache)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<GetDepartmentsDictionaryResponse, Errors>> Handle(
        GetDepartmentsDictionaryQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            conditions.Add("d.name ILIKE @search");
            parameters.Add("search", $"%{query.Search}%");
        }

        conditions.Add("d.is_active = true");

        parameters.Add("offset", (query.Page - 1) * query.PageSize, DbType.Int32);
        parameters.Add("page_size", query.PageSize, DbType.Int32);

        string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

        int totalItems = 0;

        var items = await connection
            .QueryAsync<DictionaryItemResponse, int, DictionaryItemResponse>(
                $"""
                 SELECT 
                     d.id,
                     d.name,
                     
                     CAST(COUNT(*) OVER() AS INT) AS total_count
                 FROM departments d
                 {whereClause}
                 ORDER BY d.created_at DESC
                 LIMIT @page_size OFFSET @offset;
                 """,
                splitOn: "total_count",
                map: (position, count) =>
                {
                    totalItems = count;
                    return position;
                },
                param: parameters);

        _logger.LogInformation("Departments was successfully founded!");

        return new GetDepartmentsDictionaryResponse(
            items.ToList(),
            totalItems,
            query.Page,
            query.PageSize);
    }
}