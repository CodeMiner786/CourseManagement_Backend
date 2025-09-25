using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Core.Entities;

public class Role
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string RoleName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}