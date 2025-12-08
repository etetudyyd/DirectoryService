using DirectoryService.Entities;
using DirectoryService.ValueObjects.ConnectionEntities;
using DirectoryService.ValueObjects.Department;
using DirectoryService.ValueObjects.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Configurations;

/// <summary>
/// DepartmentLocationConfiguration - configuration file for building table "departments_locations". This table
/// connects such tables as "departments" and "locations". Delete type "Restricted".
/// </summary>
public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_locations");

        builder.HasKey(l => l.Id)
            .HasName("pk_department_location");

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new DepartmentLocationId(value));

        builder.Property(x => x.DepartmentId)
            .HasColumnName("department_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new DepartmentId(value));

        builder.Property(x => x.LocationId)
            .HasColumnName("location_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new LocationId(value));

        /*builder.HasOne(dl => dl.Department)
            .WithMany(d => d.DepartmentLocations)
            .HasForeignKey(dl => dl.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dl => dl.Location)
            .WithMany(l => l.DepartmentLocations)
            .HasForeignKey(dl => dl.LocationId)
            .OnDelete(DeleteBehavior.Restrict);*/
    }
}