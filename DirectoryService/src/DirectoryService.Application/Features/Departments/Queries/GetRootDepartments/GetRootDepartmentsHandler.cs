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

        const string sql = @"WITH roots AS (
                                SELECT 
                                    d.id, 
                                    d.name,
                                    d.identifier,
                                    d.parent_id AS ""parentId"",
                                    d.created_at AS ""createdAt""                                
                                FROM departments d
                                WHERE parent_id IS NULL
                                ORDER BY d.created_at
                                OFFSET @Offset LIMIT @Limit
                            )
                            (
	                            SELECT 
		                            id,
		                            ""name"",
		                            identifier,
		                            ""parentId"",
		                            ""createdAt"", 
		                            EXISTS(
			                            SELECT 1 
			                            FROM departments d 
			                            WHERE d.parent_id = roots.id 
			                            OFFSET @Prefetch LIMIT 1
		                            ) AS ""hasMoreChildren""
	                            FROM roots
                            )        
                            UNION ALL
                            (
                            SELECT 
	                            c.id,
	                            c.""name"",
	                            c.identifier,
	                            c.parent_id,
	                            c.created_at,
	                            FALSE ""hasMoreChildren""
                            FROM roots
                            CROSS JOIN LATERAL (
                                SELECT
    	                            d.id,
                                    d.name,
                                    d.identifier,
                                    d.parent_id,
                                    d.created_at
                                FROM departments d
                                WHERE d.parent_id = roots.id
                                LIMIT @Prefetch
                                ) AS c
                            );";

        var dynamicParameters = new DynamicParameters();

        dynamicParameters.Add("Prefetch", query.DepartmentsRequest.Prefetch);
        dynamicParameters.Add("Offset", (query.DepartmentsRequest.Page - 1) * query.DepartmentsRequest.PageSize);
        dynamicParameters.Add("Limit", query.DepartmentsRequest.PageSize);

        var result = await connection
            .QueryAsync<DepartmentPrefetchResponse>(
                sql,
                param: dynamicParameters);

        _logger.LogInformation("GetRootDepartmentsWithChilds was successfully executed.");
        return new GetRootDepartmentsResponse(result.ToList());
    }

    /*public async Task<Result<GetRootDepartmentsResponse, Errors>> Handle(
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
                    SELECT d.id,
                           d.name,
                           d.identifier,
                           d.path,
                           d.parent_id,
                           d.is_active,
                           d.created_at,
                           d.update_at
                    FROM departments d
                    WHERE d.parent_id IS NULL
                    ORDER BY d.created_at
                    OFFSET @offset LIMIT @root_limit
                )
                SELECT
                    r.id,
                    r.name,
                    r.identifier,
                    r.path,
                    r.parent_id,
                    r.is_active,
                    r.created_at,
                    r.update_at,
                    (EXISTS(
                        SELECT 1 FROM departments
                        WHERE parent_id = r.id
                        OFFSET @child_limit LIMIT 1
                    ))::bool AS has_more_children
                FROM roots r

                UNION ALL

                SELECT
                    c.id,
                    c.name,
                    c.identifier,
                    c.path,
                    c.parent_id,
                    c.is_active,
                    c.created_at,
                    c.update_at,
                    (EXISTS(
                        SELECT 1 FROM departments WHERE parent_id = c.id
                    ))::bool AS has_more_children
                FROM roots r
                CROSS JOIN LATERAL (
                    SELECT
                        d.id,
                        d.name,
                        d.identifier,
                        d.path,
                        d.parent_id,
                        d.is_active,
                        d.created_at,
                        d.update_at
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
    }*/
}