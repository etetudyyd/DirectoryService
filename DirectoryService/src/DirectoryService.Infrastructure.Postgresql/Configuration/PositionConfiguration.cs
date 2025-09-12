using DevQuestions.Domain;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Entities.AdjacentEntities;
using DevQuestions.Domain.ValueObjects.PositionVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgresql.Configuration;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(l => l.Id)
            .HasName("pk_position");

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new PositionId(value));

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(LengthConstants.MAX_LENGTH_POSITION_NAME)
            .HasConversion(
                name => name.Value,
                value => PositionName.Create(value).Value);

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(LengthConstants.MAX_LENGTH_DESCRIPTION)
            .HasConversion(
                name => name.Value,
                value => PositionDescription.Create(value).Value);

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("update_at");

        builder.HasMany(x => x.DepartmentPositions)
            .WithOne()
            .HasForeignKey(x => x.PositionId);

    }
}