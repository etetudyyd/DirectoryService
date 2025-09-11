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
            builder.ToTable("Department");

            builder.HasKey(d => d.Id)
                .HasName("id");

            builder.Property(p => p.Id)
                .HasConversion(
                    id => id.Value,
                    value => new DepartmentId(value));

            builder.Property(p => p.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(LengthConstants.MAX_LENGTH_DEPARTMENT_NAME)
                .HasConversion(
                    name => name.Value,
                    value => new DepartmentName(value));

            builder.Property(p => p.Identifier)
                .HasColumnName("identifier")
                .IsRequired()
                .HasMaxLength(LengthConstants.MAX_LENGTH_DEPARTMENT_IDENTIFIER);
        }

}