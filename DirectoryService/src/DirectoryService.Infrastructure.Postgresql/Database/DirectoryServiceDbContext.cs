using DevQuestions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DirectoryService.Infrastructure.Postgresql.Database;

public class DirectoryServiceDbContext(string? connectionString) : DbContext
{
    public DbSet<Department> Departments { get; set; }

    public DbSet<DepartmentLocation> DepartmentLocations { get; set; }

    public DbSet<DepartmentPosition> DepartmentPositions { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<Position> Positions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);
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