using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Infrastructure.Data;

namespace CourseManagementSystem.Infrastructure.Repositories;

public class EnrollmentRepository : BaseRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByUserAsync(int userId)
    {
        return await _dbSet.Include(e => e.Course)
                            .ThenInclude(c => c.Category)
                          .Include(e => e.User)
                          .Where(e => e.UserId == userId)
                          .ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(int courseId)
    {
        return await _dbSet.Include(e => e.Course)
                          .Include(e => e.User)
                          .Where(e => e.CourseId == courseId)
                          .ToListAsync();
    }

    public async Task<Enrollment?> GetEnrollmentWithDetailsAsync(int id)
    {
        return await _dbSet.Include(e => e.Course)
                            .ThenInclude(c => c.Category)
                          .Include(e => e.User)
                          .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsWithDetailsAsync()
    {
        return await _dbSet.Include(e => e.Course)
                            .ThenInclude(c => c.Category)
                          .Include(e => e.User)
                          .ToListAsync();
    }

    public async Task<bool> IsUserEnrolledInCourseAsync(int userId, int courseId)
    {
        return await _dbSet.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    }
}