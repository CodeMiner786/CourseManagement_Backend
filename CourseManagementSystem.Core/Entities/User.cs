using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Core.Entities;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(250)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<Course> InstructorCourses { get; set; } = new List<Course>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}