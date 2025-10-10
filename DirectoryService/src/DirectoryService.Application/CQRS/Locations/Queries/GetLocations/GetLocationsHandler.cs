using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Abstractions.Queries;
using DirectoryService.Application.Database.IQueries;
using DirectoryService.Application.Extentions;
using DirectoryService.Contracts.Locations.Responses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.CQRS.Locations.Queries.GetLocations;

public class GetLocationsHandler : IQueryHandler<GetLocationsResponse, GetLocationsQuery>
{
    private readonly IValidator<GetLocationsQuery> _validator;
    private readonly ILogger<GetLocationsHandler> _logger;
    private readonly IReadDbContext _readDbContext;

    public GetLocationsHandler(
        ILogger<GetLocationsHandler> logger,
        IValidator<GetLocationsQuery> validator,
        IReadDbContext readDbContext)
    {
        _logger = logger;
        _validator = validator;
        _readDbContext = readDbContext;
    }

    public async Task<Result<GetLocationsResponse, Errors>> Handle(GetLocationsQuery query, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors(); // перетворюємо в твій стандартний формат помилки

        // Безпечна робота з Ids
        var ids = query.Ids?.ToArray() ?? [];

        // Завантаження даних з БД
        var locations = await _readDbContext.LocationsRead
            .Where(l => ids.Contains(l.Id.Value))
            .ToArrayAsync(cancellationToken);

        var locationIds = locations.Select(l => l.Id.Value).ToArray();

        var response = new GetLocationsResponse(locationIds, query.Search, query.IsActive);

        return Result.Success<GetLocationsResponse, Errors>(response);
    }

}