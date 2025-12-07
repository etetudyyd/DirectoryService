using DirectoryService.Entities;
using DirectoryService.ValueObjects.Position;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Configurations;

/// <summary>
/// PositionConfiguration - configuration file for building table "positions". Delete type "Restricted".
/// </summary>
public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(l => l.Id)
            .HasName("pk_position");

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new PositionId(value));

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(Constants.MAX_LENGTH_POSITION_NAME)
            .HasConversion(
                name => name.Value,
                value => PositionName.Create(value).Value);

        builder.HasIndex(l => l.Name).IsUnique();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(Constants.MAX_LENGTH_DESCRIPTION)
            .HasConversion(
                name => name.Value,
                value => Description.Create(value).Value);

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

        builder.HasMany(x => x.DepartmentPositions)
            .WithOne(x => x.Position)
            .HasForeignKey(x => x.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}