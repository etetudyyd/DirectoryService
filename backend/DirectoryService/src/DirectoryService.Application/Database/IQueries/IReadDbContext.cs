using DevQuestions.Domain.Entities;

namespace DirectoryService.Application.Database.IQueries;

public interface IReadDbContext
{
    IQueryable<Department> DepartmentsRead { get; }

    IQueryable<Location> LocationsRead { get; }

    IQueryable<Position> PositionsRead { get; }

    IQueryable<DepartmentLocation> DepartmentLocationsRead { get; }

    IQueryable<DepartmentPosition> DepartmentPositionsRead { get; }
}