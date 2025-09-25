using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Core.DTOs.User;
public class UpdateUserDto
{
    [Required]
    [StringLength(250)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(250)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }
}