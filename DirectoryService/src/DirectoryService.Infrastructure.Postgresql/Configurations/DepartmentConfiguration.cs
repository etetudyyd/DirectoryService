using DevQuestions.Domain;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Path = DevQuestions.Domain.ValueObjects.DepartmentVO.Path;

namespace DirectoryService.Infrastructure.Postgresql.Configurations;

/// <summary>
/// DepartmentConfiguration - configuration file for building table "departments". Delete type "Restricted".
/// </summary>
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
            .HasMaxLength(Constants.MAX_LENGTH_DEPARTMENT_NAME)
            .HasConversion(
                name => name.Value,
                value => DepartmentName.Create(value).Value);

            builder.HasIndex(l => l.Name).IsUnique();

            builder.Property(p => p.Identifier)
            .HasColumnName("identifier")
            .IsRequired()
            .HasMaxLength(Constants.MAX_LENGTH_DEPARTMENT_IDENTIFIER)
            .HasConversion(
                name => name.Value,
                value => Identifier.Create(value).Value);

            builder.Property(p => p.ParentId)
                .HasColumnName("parent_id");

            builder.Property(x => x.Path)
                .HasColumnName("path")
                .HasColumnType("ltree")
                .IsRequired()
                .HasMaxLength(Constants.MAX_LENGTH_DEPARTMENT_PATH)
                .HasConversion(
                    value => value.Value,
                    value => Path.CreateForDb(value));

            builder.HasIndex(x => x.Path)
                .HasMethod("gist")
                .HasDatabaseName("idx_departments_path");

            builder.Property(p => p.Depth)
            .HasColumnName("depth")
            .IsRequired();

            builder.Property(p => p.ChildrenCount)
                .HasColumnName("children_count")
                .IsRequired();

            builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

            builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

            builder.Property(p => p.UpdatedAt)
            .HasColumnName("update_at");

            builder.Property(p => p.DeletedAt)
                .HasColumnName("deleted_at")
                .IsRequired(false);

            builder.HasMany(x => x.ChildrenDepartments)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.DepartmentLocations)
            .WithOne(x => x.Department)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.DepartmentPositions)
            .WithOne(x => x.Department)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        }

}