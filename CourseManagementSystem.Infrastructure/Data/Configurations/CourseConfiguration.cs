using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagementSystem.Core.Entities;

namespace CourseManagementSystem.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(c => c.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.DiscountPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.DateCreated)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(c => c.DateUpdated)
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(c => c.Category)
            .WithMany(cc => cc.Courses)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Instructor)
            .WithMany(u => u.InstructorCourses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}