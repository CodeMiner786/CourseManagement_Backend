using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourseManagementSystem.Core.Entities;

namespace CourseManagementSystem.Infrastructure.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.PaymentAmount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(e => e.Discount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(e => e.PaymentTotal)
            .HasColumnType("decimal(18,2)")
            .HasComputedColumnSql("[PaymentAmount] - [Discount]", stored: true);

        builder.Property(e => e.EnrollmentDate)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(e => e.EnrollmentStatus)
            .HasConversion<int>();

        builder.Property(e => e.PaymentStatus)
            .HasConversion<int>();

        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}