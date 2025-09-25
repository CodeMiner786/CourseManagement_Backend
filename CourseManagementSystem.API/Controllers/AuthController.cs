using Microsoft.AspNetCore.Mvc;
using CourseManagementSystem.Core.DTOs.Auth;
using CourseManagementSystem.Infrastructure.Services;

namespace CourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>JWT token response</returns>
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(loginDto);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        return Ok(result);
    }

    /// <summary>
    /// User registration
    /// </summary>
    /// <param name="registerDto">Registration details</param>
    /// <returns>JWT token response</returns>
    [HttpPost("register")]
    public async Task<ActionResult<TokenResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(registerDto);

        if (result == null)
        {
            return BadRequest(new { message = "User with this email or phone number already exists, or invalid role" });
        }

        return CreatedAtAction(nameof(Register), result);
    }
}