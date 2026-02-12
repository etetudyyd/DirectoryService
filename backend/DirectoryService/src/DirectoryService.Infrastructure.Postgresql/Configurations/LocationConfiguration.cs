using DirectoryService.Entities;
using DirectoryService.ValueObjects.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Configurations;

/// <summary>
/// LocationConfiguration - configuration file for building table "locations". Delete type "Restricted".
/// </summary>
public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id)
            .HasName("pk_location");

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new LocationId(value));

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(Constants.MAX_LENGTH_POSITION_NAME)
            .HasConversion(
                name => name.Value,
                value => LocationName.Create(value).Value);

        builder.HasIndex(l => l.Name).IsUnique();

        builder.ComplexProperty(p => p.Address, a => {
            a.Property(address => address.PostalCode)
                .HasMaxLength(Constants.Address.MAX_LENGTH_ADDRESS_POSTAL_CODE)
                .HasColumnName("postal_code");

            a.Property(address => address.Region)
                .HasMaxLength(Constants.Address.MAX_LENGTH_ADDRESS_REGION)
                .HasColumnName("region");

            a.Property(address => address.City)
                .IsRequired()
                .HasMaxLength(Constants.Address.MAX_LENGTH_ADDRESS_CITY)
                .HasColumnName("city");

            a.Property(address => address.Street)
                .IsRequired()
                .HasMaxLength(Constants.Address.MAX_LENGTH_ADDRESS_STREET)
                .HasColumnName("street");

            a.Property(address => address.House)
                .IsRequired()
                .HasMaxLength(Constants.Address.MAX_LENGTH_ADDRESS_HOUSE)
                .HasColumnName("house");

            a.Property(address => address.Apartment)
                .IsRequired()
                .HasMaxLength(Constants.Address.MAX_LENGTH_ADDRESS_APARTMENT)
                .HasColumnName("apartment");
        });

        builder.Property(p => p.Timezone)
            .HasColumnName("timezone")
            .HasConversion(
                name => name.Value,
                value => Timezone.Create(value).Value);

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(p => p.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasMany(x => x.DepartmentLocations)
            .WithOne(x => x.Location)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}