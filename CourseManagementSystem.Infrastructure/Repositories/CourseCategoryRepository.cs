using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Infrastructure.Data;

namespace CourseManagementSystem.Infrastructure.Repositories;

public class CourseCategoryRepository : BaseRepository<CourseCategory>, ICourseCategoryRepository
{
    public CourseCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<CourseCategory?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(cc => cc.Name == name);
    }

    public async Task<IEnumerable<CourseCategory>> GetCategoriesWithCoursesAsync()
    {
        return await _dbSet.Include(cc => cc.Courses)
                          .ToListAsync();
    }
}