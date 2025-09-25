using CourseManagementSystem.Core.DTOs.Auth;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Interfaces;

namespace CourseManagementSystem.Infrastructure.Services;

public interface IAuthService
{
    Task<TokenResponseDto?> LoginAsync(LoginDto loginDto);
    Task<TokenResponseDto?> RegisterAsync(RegisterDto registerDto);
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IPasswordService passwordService, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);

        if (user == null || !_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return null;
        }

        var token = _jwtService.GenerateToken(user);
        return _jwtService.CreateTokenResponse(user, token);
    }

    public async Task<TokenResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        // Check if user already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return null;
        }

        // Check if phone number already exists
        var existingPhone = await _unitOfWork.Users.GetByPhoneNumberAsync(registerDto.PhoneNumber);
        if (existingPhone != null)
        {
            return null;
        }

        // Verify role exists
        var role = await _unitOfWork.Roles.GetByIdAsync(registerDto.RoleId);
        if (role == null)
        {
            return null;
        }

        var user = new User
        {
            FullName = registerDto.FullName,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            PasswordHash = _passwordService.HashPassword(registerDto.Password),
            RoleId = registerDto.RoleId,
            DateCreated = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Get user with role for token generation
        var userWithRole = await _unitOfWork.Users.GetUserWithRoleAsync(user.Id);
        if (userWithRole == null)
        {
            return null;
        }

        var token = _jwtService.GenerateToken(userWithRole);
        return _jwtService.CreateTokenResponse(userWithRole, token);
    }
}