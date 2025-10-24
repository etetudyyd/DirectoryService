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
        parameters.Add("parent_id", query.Request.ParentId);
        parameters.Add("offset", (query.Request.Page - 1) * query.Request.PageSize, DbType.Int32);
        parameters.Add("page_size", query.Request.PageSize, DbType.Int32);

        var departments = await connection.QueryAsync<DepartmentPrefetchResponse>(
            """
             SELECT
                 d.id AS "Id",
                 d.name AS "Name",
                 d.identifier AS "Identifier",
                 d.path AS "Path",
                 d.parent_id AS "ParentId",
                 d.is_active AS "IsActive",
                 d.created_at AS "CreatedAt",
                 d.update_at AS "UpdateAt"
             FROM departments d
             WHERE d.parent_id = @parentId
             ORDER BY d.created_at DESC
             LIMIT @page_size OFFSET @offset;
             """,
            param: parameters);

        _logger.LogInformation("Departments was successfully founded!");

        return new GetChildrenDepartmentsResponse(departments.ToList());
    }
}