using CourseManagementSystem.Core.Entities;

namespace CourseManagementSystem.Core.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string roleName);
}