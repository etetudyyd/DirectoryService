using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Application.Database.IQueries;
using DirectoryService.Application.Extentions;
using DirectoryService.Contracts.Departments.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Features.Departments.Queries.GetChildrenDepartments;

public class GetChildrenDepartmentsHandler : IQueryHandler<GetChildrenDepartmentsResponse, GetChildrenDepartmentsQuery>
{
    private readonly IDapperConnectionFactory _connectionFactory;
    private readonly IValidator<GetChildrenDepartmentsQuery> _validator;
    private readonly ILogger<GetChildrenDepartmentsHandler> _logger;

    public GetChildrenDepartmentsHandler(
        IDapperConnectionFactory connectionFactory,
        IValidator<GetChildrenDepartmentsQuery> validator,
        ILogger<GetChildrenDepartmentsHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<GetChildrenDepartmentsResponse, Errors>> Handle(
        GetChildrenDepartmentsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("ParentId", query.Request.ParentId);
        parameters.Add("Offset", (query.Request.Page - 1) * query.Request.PageSize, DbType.Int32);
        parameters.Add("Limit", query.Request.PageSize, DbType.Int32);

        var departments = await connection.QueryAsync<DepartmentPrefetchResponse>(
            """
            WITH child_departments AS (
                SELECT
                    d.id,
                    d.name,
                    d.identifier,
                    d.path,
                    d.parent_id AS ParentId,
                    d.is_active AS IsActive,
                    d.created_at AS CreatedAt,
                    d.update_at AS UpdateAt
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
                UpdateAt,
                EXISTS(
                    SELECT 1
                    FROM departments d
                    WHERE d.parent_id = child_departments.id
                ) AS HasMoreChildren
            FROM child_departments;
            """,
            param: parameters);

        _logger.LogInformation("Departments was successfully founded!");

        return new GetChildrenDepartmentsResponse(departments.ToList());
    }
}