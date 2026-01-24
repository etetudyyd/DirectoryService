using Core.Abstractions;
using Core.Validation;
using CSharpFunctionalExtensions;
using DirectoryService.Database.IQueries;
using DirectoryService.Locations;
using DirectoryService.Locations.Responses;
using DirectoryService.ValueObjects.Location;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.SharedKernel;

namespace DirectoryService.Features.Locations.Queries.GetLocationById;

public class GetLocationByIdHandler : IQueryHandler<GetLocationByIdResponse, GetLocationByIdQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IValidator<GetLocationByIdQuery> _validator;
    private readonly ILogger<GetLocationByIdHandler> _logger;

    public GetLocationByIdHandler(
        IReadDbContext readDbContext,
        ILogger<GetLocationByIdHandler> logger,
        IValidator<GetLocationByIdQuery> validator)
    {
        _readDbContext = readDbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetLocationByIdResponse, Errors>> Handle(GetLocationByIdQuery query, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var location = await _readDbContext.LocationsRead
            .FirstOrDefaultAsync(l => l.Id == new LocationId(query.Id), cancellationToken);

        if (location is null)
        {
            return Error.NotFound("location.not.found", "Location not found")
                .ToErrors();
        }

        _logger.LogInformation("Get location by id: {Id}", location.Id);

        return new GetLocationByIdResponse(
            new LocationDto
        {
            Id = location.Id.Value,
            Name = location.Name.Value,
            Address = new AddressDto
            {
                PostalCode = location.Address.PostalCode,
                Region = location.Address.Region,
                City = location.Address.City,
                Street = location.Address.Street,
                House = location.Address.House,
                Apartment = location.Address.Apartment,
            },
            TimeZone = location.Timezone.Value,
        });
    }
}