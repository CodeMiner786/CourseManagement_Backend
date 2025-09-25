using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CourseManagementSystem.Core.DTOs.User;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Infrastructure.Services;

namespace CourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;

    public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPasswordService passwordService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordService = passwordService;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of users</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
        return Ok(userDtos);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _unitOfWork.Users.GetUserWithRoleAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        // Users can only view their own profile unless they're admin
        if (!User.IsInRole("Admin") && user.Id.ToString() != User.FindFirst("UserId")?.Value)
        {
            return Forbid();
        }

        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="createUserDto">User creation details</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if user already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(createUserDto.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "User with this email already exists" });
        }

        var existingPhone = await _unitOfWork.Users.GetByPhoneNumberAsync(createUserDto.PhoneNumber);
        if (existingPhone != null)
        {
            return BadRequest(new { message = "User with this phone number already exists" });
        }

        // Verify role exists
        var role = await _unitOfWork.Roles.GetByIdAsync(createUserDto.RoleId);
        if (role == null)
        {
            return BadRequest(new { message = "Invalid role" });
        }

        var user = _mapper.Map<User>(createUserDto);
        user.PasswordHash = _passwordService.HashPassword(createUserDto.Password);
        user.DateCreated = DateTime.UtcNow;

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var createdUser = await _unitOfWork.Users.GetUserWithRoleAsync(user.Id);
        var userDto = _mapper.Map<UserDto>(createdUser);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
    }

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="updateUserDto">User update details</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Users can only update their own profile unless they're admin
        if (!User.IsInRole("Admin") && user.Id.ToString() != User.FindFirst("UserId")?.Value)
        {
            return Forbid();
        }

        // Check if email is being changed and if it already exists
        if (user.Email != updateUserDto.Email)
        {
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(updateUserDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }
        }

        // Check if phone number is being changed and if it already exists
        if (user.PhoneNumber != updateUserDto.PhoneNumber)
        {
            var existingPhone = await _unitOfWork.Users.GetByPhoneNumberAsync(updateUserDto.PhoneNumber);
            if (existingPhone != null)
            {
                return BadRequest(new { message = "User with this phone number already exists" });
            }
        }

        // Only admin can change roles
        if (!User.IsInRole("Admin") && user.RoleId != updateUserDto.RoleId)
        {
            return Forbid("Only administrators can change user roles");
        }

        // Verify role exists if being changed
        if (user.RoleId != updateUserDto.RoleId)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(updateUserDto.RoleId);
            if (role == null)
            {
                return BadRequest(new { message = "Invalid role" });
            }
        }

        _mapper.Map(updateUserDto, user);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        var updatedUser = await _unitOfWork.Users.GetUserWithRoleAsync(user.Id);
        var userDto = _mapper.Map<UserDto>(updatedUser);

        return Ok(userDto);
    }

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _unitOfWork.Users.Remove(user);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Get users by role
    /// </summary>
    /// <param name="roleName">Role name</param>
    /// <returns>List of users with specified role</returns>
    [HttpGet("by-role/{roleName}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(string roleName)
    {
        var users = await _unitOfWork.Users.GetUsersByRoleAsync(roleName);
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
        return Ok(userDtos);
    }
}