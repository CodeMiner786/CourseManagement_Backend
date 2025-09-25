// File: CourseManagementSystem.Infrastructure/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Infrastructure.Data;

namespace CourseManagementSystem.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.Include(u => u.Role)
                          .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _dbSet.Include(u => u.Role)
                          .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<User?> GetUserWithRoleAsync(int id)
    {
        return await _dbSet.Include(u => u.Role)
                          .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
    {
        return await _dbSet.Include(u => u.Role)
                          .Where(u => u.Role.RoleName == roleName)
                          .ToListAsync();
    }
}

