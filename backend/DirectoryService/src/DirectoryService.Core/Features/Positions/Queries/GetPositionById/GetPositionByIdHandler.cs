using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IQueries;
using DirectoryService.Positions;
using DirectoryService.Positions.Responses;
using DirectoryService.ValueObjects.Position;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Positions.Queries.GetPositionById;

public class GetPositionByIdHandler : IQueryHandler<GetPositionByIdResponse, GetPositionByIdQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IValidator<GetPositionByIdQuery> _validator;
    private readonly ILogger<GetPositionByIdHandler> _logger;

    public GetPositionByIdHandler(IReadDbContext readDbContext, IValidator<GetPositionByIdQuery> validator, ILogger<GetPositionByIdHandler> logger)
    {
        _readDbContext = readDbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetPositionByIdResponse, Errors>> Handle(
        GetPositionByIdQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var positionDto = await _readDbContext.PositionsRead
            .Where(p => p.Id == new PositionId(query.Id))
            .Select(p => new PositionDetailsDto
            {
                Id = p.Id.Value,
                Name = p.Name.Value,
                Description = p.Description.Value,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                DeletedAt = p.DeletedAt,
                DepartmentCount = p.DepartmentPositions.Count,
                Departments = p.DepartmentPositions
                    .Where(dp => dp.Department.DeletedAt == null) // Проверка на soft delete
                    .Select(dp => new DictionaryItemResponse
                    {
                        Id = dp.DepartmentId.Value,
                        Name = dp.Department!.Name.Value,
                    })
                    .ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (positionDto is null)
        {
            return Error.NotFound("position.not.found", "Position not found")
                .ToErrors();
        }

        _logger.LogInformation("Get position by id: {Id}", query.Id);

        return new GetPositionByIdResponse(positionDto);
    }
}