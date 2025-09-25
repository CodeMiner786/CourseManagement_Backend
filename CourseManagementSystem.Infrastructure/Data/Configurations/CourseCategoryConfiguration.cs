using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagementSystem.Core.Entities;

namespace CourseManagementSystem.Infrastructure.Data.Configurations;

public class CourseCategoryConfiguration : IEntityTypeConfiguration<CourseCategory>
{
    public void Configure(EntityTypeBuilder<CourseCategory> builder)
    {
        builder.ToTable("CourseCategories");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.Id)
            .ValueGeneratedOnAdd();

        builder.Property(cc => cc.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(cc => cc.Name)
            .IsUnique();

        builder.Property(cc => cc.Description)
            .HasMaxLength(500);
    }
}