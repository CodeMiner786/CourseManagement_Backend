using CourseManagementSystem.Core.Entities;

namespace CourseManagementSystem.Core.Interfaces;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<IEnumerable<Enrollment>> GetEnrollmentsByUserAsync(int userId);
    Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(int courseId);
    Task<Enrollment?> GetEnrollmentWithDetailsAsync(int id);
    Task<IEnumerable<Enrollment>> GetEnrollmentsWithDetailsAsync();
    Task<bool> IsUserEnrolledInCourseAsync(int userId, int courseId);
}