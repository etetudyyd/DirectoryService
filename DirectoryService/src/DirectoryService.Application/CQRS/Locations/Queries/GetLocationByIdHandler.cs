using DevQuestions.Domain.Entities;
using DevQuestions.Domain.ValueObjects.LocationVO;
using DirectoryService.Application.Database.IQueries;
using DirectoryService.Contracts.Locations;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Application.CQRS.Locations.Queries;

/*public class GetLocationByIdHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetLocationByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<GetLocationDto?> Handle(GetLocationByIdDto query, CancellationToken cancellationToken)
    {
        var location = await _readDbContext.LocationsRead
            .FirstOrDefaultAsync(l => l.Id == new LocationId(query.LocationId), cancellationToken);

        if (location is null)
        {
            return null;
        }

        return new GetLocationDto
        {
            Id = location.Id.Value,
            Name = location.Name.Value,
            Address = location.Address,
            Timezone = location.Timezone.Value,
        };
    }
}*/