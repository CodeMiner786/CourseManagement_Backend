using System.ComponentModel.DataAnnotations;
using CourseManagementSystem.Core.Enums;

namespace CourseManagementSystem.Core.DTOs.Enrollment;

public class EnrollmentDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal PaymentTotal { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public EnrollmentStatus EnrollmentStatus { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}