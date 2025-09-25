
using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Core.DTOs.CourseCategory;

public class CourseCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CourseCount { get; set; }
}