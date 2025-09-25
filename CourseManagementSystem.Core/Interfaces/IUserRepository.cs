using CourseManagementSystem.Core.Entities;

namespace CourseManagementSystem.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<User?> GetUserWithRoleAsync(int id);
    Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
}