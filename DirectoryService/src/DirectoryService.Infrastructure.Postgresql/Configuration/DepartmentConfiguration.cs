using DevQuestions.Domain;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgresql.Configuration;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
            builder.ToTable("departments");

            builder.HasKey(d => d.Id);

            builder.Property(p => p.Id)
            .HasColumnName("id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new DepartmentId(value));

            builder.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(LengthConstants.MAX_LENGTH_DEPARTMENT_NAME)
            .HasConversion(name => name.Value, value => DepartmentName.Create(value).Value);

            builder.Property(p => p.Identifier)
            .HasColumnName("identifier")
            .IsRequired()
            .HasMaxLength(LengthConstants.MAX_LENGTH_DEPARTMENT_IDENTIFIER)
            .HasConversion(
                name => name.Value, 
                value => Identifier.Create(value).Value);

            builder.Property(p => p.ParentId)
            .HasColumnName("parent_id")
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value != null ? new DepartmentId(value.Value) : null);

            builder.Property(p => p.Path)
            .HasColumnName("path")
            .IsRequired()
            .HasMaxLength(LengthConstants.MAX_LENGTH_DEPARTMENT_PATH)
            .HasConversion(
                name => name.Value,
                value => DepartmentPath.Create(value).Value);

            builder.Property(p => p.Depth)
            .HasColumnName("depth")
            .IsRequired();

            builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

            builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

            builder.Property(p => p.UpdatedAt)
            .HasColumnName("update_at");

            builder.HasMany(x => x.Children)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Locations)
            .WithOne()
            .HasForeignKey(x => x.DepartmentId);

            builder.HasMany(x => x.Positions)
            .WithOne()
            .HasForeignKey(x => x.DepartmentId);
        }

}