using CourseManagementSystem.Core.Entities;

namespace CourseManagementSystem.Core.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId);
    Task<IEnumerable<Course>> GetCoursesByInstructorAsync(int instructorId);
    Task<Course?> GetCourseWithDetailsAsync(int id);
    Task<IEnumerable<Course>> GetCoursesWithDetailsAsync();
}