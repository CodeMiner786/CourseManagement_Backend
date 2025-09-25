using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Infrastructure.Data;

namespace CourseManagementSystem.Infrastructure.Repositories;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task GetByIdAsync(int? roleId)
    {
        throw new NotImplementedException();
    }

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.RoleName == roleName);
    }
}