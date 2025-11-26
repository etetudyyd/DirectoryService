using CSharpFunctionalExtensions;
using Dapper;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Application.Database.IQueries;
using DirectoryService.Application.Extentions;
using DirectoryService.Contracts.Departments.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Features.Departments.Queries.GetTopDepartmentsByPositions;

public class GetTopDepartmentsByPositionsHandler : IQueryHandler<GetTopDepartmentsByPositionsResponse, GetTopDepartmentsByPositionsQuery>
{
    private readonly IDapperConnectionFactory _connectionFactory;
    private readonly IValidator<GetTopDepartmentsByPositionsQuery> _validator;
    private readonly ILogger<GetTopDepartmentsByPositionsHandler> _logger;


    public GetTopDepartmentsByPositionsHandler(
        IDapperConnectionFactory connectionFactory,
        ILogger<GetTopDepartmentsByPositionsHandler> logger,
        IValidator<GetTopDepartmentsByPositionsQuery> validator)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<GetTopDepartmentsByPositionsResponse, Errors>> Handle(
        GetTopDepartmentsByPositionsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var departments = await connection
            .QueryAsync<DepartmentResponse>(
            """
            SELECT
                d.id AS "Id",
                d.name AS "Name",
                d.identifier AS "Identifier",
                d.path AS "Path",
                d.parent_id AS "ParentId",
                d.is_active AS "IsActive",
                d.created_at AS "CreatedAt",
                d.updated_at AS "UpdatedAt",
                COUNT(dp.position_id) AS "PositionCount"
            FROM departments d
            LEFT JOIN department_positions dp ON dp.department_id = d.id
            GROUP BY d.id, d.name, d.identifier, d.path, d.parent_id, d.is_active, d.created_at, d.updated_at
            ORDER BY "PositionCount" DESC
            LIMIT 5;
            """);

        _logger.LogInformation("Departments was successfully founded!");

        return new GetTopDepartmentsByPositionsResponse(departments.ToList());
    }
}