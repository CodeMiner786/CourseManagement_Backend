using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Core.DTOs.Course
{
    public class UpdateCourseDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount price must be greater than or equal to 0")]
        public decimal? DiscountPrice { get; set; }

        public int? InstructorId { get; set; }
    }
}
