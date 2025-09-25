using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Core.DTOs.User;

public class CreateUserDto
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
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }
}