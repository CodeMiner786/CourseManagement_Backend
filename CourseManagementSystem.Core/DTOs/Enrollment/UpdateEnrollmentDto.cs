using CourseManagementSystem.Core.Enums;
using System.ComponentModel.DataAnnotations;

public class UpdateEnrollmentDto
{
    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Payment amount must be greater than or equal to 0")]
    public decimal PaymentAmount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Discount must be greater than or equal to 0")]
    public decimal Discount { get; set; }

    public DateTime? PaymentDate { get; set; }

    [Required]
    public EnrollmentStatus EnrollmentStatus { get; set; }

    [Required]
    public PaymentStatus PaymentStatus { get; set; }
}