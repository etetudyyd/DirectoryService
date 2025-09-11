using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Entities.AdjacentEntities;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgresql;

public class DirectoryServiceDbContext(string? connectionString) : DbContext
{
    private readonly string? _connectionString = connectionString;

    public DbSet<Department> Departments { get; set; }

    public DbSet<DepartmentLocation> DepartmentLocations { get; set; }

    public DbSet<DepartmentPosition> DepartmentPositions { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<Position> Positions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.EnableSensitiveDataLogging();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("DirectoryService");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DirectoryServiceDbContext).Assembly,
            type => type.FullName?.ToLower().Contains("configuration") ?? false);
    }

}