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
        parameters.Add("page_size", query.DepartmentsRequest.PageSize, DbType.Int32);
        parameters.Add("child_limit", query.DepartmentsRequest.Prefetch, DbType.Int32);

        var departments = await connection
            .QueryAsync<DepartmentPrefetchResponse>(
                """
                     WITH roots AS (SELECT d.id,
                                          d.name,
                                          d.identifier,
                                          d.path,
                                          d.parent_id,
                                          d.depth,
                                          d.children_count,
                                          d.is_active,
                                          d.created_at,
                                          d.update_at
                                   FROM departments d
                                   WHERE d.parent_id IS NULL
                                   ORDER BY d.created_at
                                   OFFSET @offset LIMIT @root_limit)
                    SELECT *, (EXISTS(SELECT 1 FROM departments 
                                               WHERE parent_id = roots.id 
                                               OFFSET @child_limit LIMIT 1)) AS has_more_children
                    FROM roots
                    
                    UNION ALL
                    
                    SELECT c.*, (EXISTS(SELECT 1 FROM departments WHERE parent_id = c.id)) AS has_more_children
                    FROM roots r
                        CROSS JOIN LATERAL (SELECT d.id,
                                                   d.name,
                                                   d.identifier,
                                                   d.path,
                                                   d.parent_id,
                                                   d.depth,
                                                   d.children_count,
                                                   d.is_active,
                                                   d.created_at,
                                                   d.update_at
                                            FROM departments d
                                            WHERE d.parent_id = r.id
                                            AND d.is_active = true
                                            ORDER BY d.created_at 
                                            LIMIT @child_limit) c;                               
                    """,
                param: parameters);

        _logger.LogInformation("Departments was successfully founded!");

        return new GetRootDepartmentsResponse(departments.ToList());
    }
}