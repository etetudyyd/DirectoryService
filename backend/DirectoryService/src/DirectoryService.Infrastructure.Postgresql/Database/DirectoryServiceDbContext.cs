using DevQuestions.Domain.Entities;
using DirectoryService.Application.Database.IQueries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DirectoryService.Infrastructure.Postgresql.Database;

/// <summary>
/// DirectoryServiceDbContext - basic realization of DbContext repository and declare DbSet contexts. Configuring ModelCreating.
/// </summary>
public class DirectoryServiceDbContext : DbContext, IReadDbContext
{
    private readonly string _connectionString;

    public DirectoryServiceDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<DepartmentLocation> DepartmentLocations => Set<DepartmentLocation>();

    public DbSet<DepartmentPosition> DepartmentPositions => Set<DepartmentPosition>();

    public IQueryable<Department> DepartmentsRead => Set<Department>().AsQueryable().AsNoTracking();

    public IQueryable<Location> LocationsRead => Set<Location>().AsQueryable().AsNoTracking();

    public IQueryable<Position> PositionsRead => Set<Position>().AsQueryable().AsNoTracking();

    public IQueryable<DepartmentLocation> DepartmentLocationsRead => Set<DepartmentLocation>().AsQueryable().AsNoTracking();

    public IQueryable<DepartmentPosition> DepartmentPositionsRead => Set<DepartmentPosition>().AsQueryable().AsNoTracking();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.EnableSensitiveDataLogging();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("DirectoryService");
        modelBuilder.HasPostgresExtension("ltree");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DirectoryServiceDbContext).Assembly,
            type => type.FullName != null &&
                    type.FullName.Contains("Configuration", StringComparison.OrdinalIgnoreCase));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties()
                         .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
            {
                property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                    v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                ));
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}