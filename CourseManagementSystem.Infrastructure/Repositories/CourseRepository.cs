using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Infrastructure.Data;

namespace CourseManagementSystem.Infrastructure.Repositories;

public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId)
    {
        return await _dbSet.Include(c => c.Category)
                          .Include(c => c.Instructor)
                          .Where(c => c.CategoryId == categoryId)
                          .ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetCoursesByInstructorAsync(int instructorId)
    {
        return await _dbSet.Include(c => c.Category)
                          .Include(c => c.Instructor)
                          .Where(c => c.InstructorId == instructorId)
                          .ToListAsync();
    }

    public async Task<Course?> GetCourseWithDetailsAsync(int id)
    {
        return await _dbSet.Include(c => c.Category)
                          .Include(c => c.Instructor)
                          .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Course>> GetCoursesWithDetailsAsync()
    {
        return await _dbSet.Include(c => c.Category)
                          .Include(c => c.Instructor)
                          .ToListAsync();
    }
}