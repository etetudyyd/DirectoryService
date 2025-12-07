using DirectoryService.Entities;

namespace DirectoryService.Database.IQueries;

public interface IReadDbContext
{
    IQueryable<Department> DepartmentsRead { get; }

    IQueryable<Location> LocationsRead { get; }

    IQueryable<Position> PositionsRead { get; }

    IQueryable<DepartmentLocation> DepartmentLocationsRead { get; }

    IQueryable<DepartmentPosition> DepartmentPositionsRead { get; }
}