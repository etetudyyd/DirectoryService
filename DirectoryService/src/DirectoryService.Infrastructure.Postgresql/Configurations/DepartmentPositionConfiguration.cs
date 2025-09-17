using DevQuestions.Domain.Entities;
using DevQuestions.Domain.ValueObjects.ConectionEntitiesVO;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DevQuestions.Domain.ValueObjects.PositionVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgresql.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_positions");

        builder.HasKey(dp => dp.Id).HasName("pk_department_position");

        builder.Property(dp => dp.Id)
            .HasColumnName("id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new DepartmentPositionId(value));

        builder.Property(dp => dp.DepartmentId)
            .HasColumnName("department_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new DepartmentId(value));
        builder.Property(dp => dp.PositionId)
            .HasColumnName("position_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new PositionId(value));
    }
}