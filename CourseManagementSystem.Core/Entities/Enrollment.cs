using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CourseManagementSystem.Core.Enums;

namespace CourseManagementSystem.Core.Entities;

public class Enrollment
{
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    public int UserId { get; set; }

    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PaymentAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PaymentTotal { get; private set; }

    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public DateTime? PaymentDate { get; set; }

    [Required]
    public EnrollmentStatus EnrollmentStatus { get; set; }

    [Required]
    public PaymentStatus PaymentStatus { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}