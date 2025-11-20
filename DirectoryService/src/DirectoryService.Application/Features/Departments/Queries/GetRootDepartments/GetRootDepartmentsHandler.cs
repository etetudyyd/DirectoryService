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

namespace DirectoryService.Application.Features.Departments.Queries.GetRootDepartments;

public class GetRootDepartmentsHandler : IQueryHandler<GetRootDepartmentsResponse, GetRootDepartmentsQuery>
{
    private readonly IDapperConnectionFactory _connectionFactory;
    private readonly IValidator<GetRootDepartmentsQuery> _validator;
    private readonly ILogger<GetRootDepartmentsHandler> _logger;

    public GetRootDepartmentsHandler(
        IDapperConnectionFactory connectionFactory,
        IValidator<GetRootDepartmentsQuery> validator,
        ILogger<GetRootDepartmentsHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetRootDepartmentsResponse, Errors>> Handle(
        GetRootDepartmentsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();

        parameters.Add("offset", (query.DepartmentsRequest.Page - 1) * query.DepartmentsRequest.PageSize, DbType.Int32);
        parameters.Add("root_limit", query.DepartmentsRequest.PageSize, DbType.Int32);
        parameters.Add("child_limit", query.DepartmentsRequest.Prefetch, DbType.Int32);

        var departments = await connection
            .QueryAsync<DepartmentPrefetchResponse>(
                """
                WITH roots AS (
                    SELECT d.id AS Id,
                           d.name AS Name,
                           d.identifier AS Identifier,
                           d.path AS PATH,
                           d.parent_id AS ParentId,
                           d.is_active AS IsActive,
                           d.created_at AS CreatedAt,
                           d.updated_at AS UpdatedAt
                    FROM departments d
                    WHERE d.parent_id IS NULL
                    ORDER BY d.created_at
                    OFFSET @offset LIMIT @root_limit
                )
                SELECT
                    r.id AS Id,
                    r.name AS Name,
                    r.identifier AS Identifier,
                    r.path AS Path,
                    r.ParentId,
                    r.IsActive,
                    r.CreatedAt,
                    r.UpdateAt,
                    (EXISTS(
                        SELECT 1 FROM departments
                        WHERE parent_id = r.id
                        OFFSET @child_limit LIMIT 1
                    ))::bool AS HasMoreChildren
                FROM roots r
                UNION ALL
                SELECT
                    c.id AS Id,
                    c.name AS Name,
                    c.identifier AS Identifier,
                    c.path AS Path,
                    c.parent_id AS ParentId,
                    c.is_active AS IsActive,
                    c.created_at AS CreatedAt,
                    c.updated_at AS UpdatedAt,
                    (EXISTS(
                        SELECT 1 FROM departments WHERE parent_id = c.id
                    ))::bool AS HasMoreChildren
                FROM roots r
                         CROSS JOIN LATERAL (
                    SELECT
                        d.id AS Id,
                        d.name AS Name,
                        d.identifier AS Identifier,
                        d.path AS Path,
                        d.parent_id,
                        d.is_active,
                        d.created_at,
                        d.updated_at
                    FROM departments d
                    WHERE d.parent_id = r.id
                      AND d.is_active = true
                    ORDER BY d.created_at
                    LIMIT @child_limit
                    ) c;
                """,
                param: parameters);

        _logger.LogInformation("Departments was successfully founded!");

        return new GetRootDepartmentsResponse(departments.ToList());
    }
}