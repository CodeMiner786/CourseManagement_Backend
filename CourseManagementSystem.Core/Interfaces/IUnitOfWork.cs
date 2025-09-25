namespace CourseManagementSystem.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICourseRepository Courses { get; }
    IEnrollmentRepository Enrollments { get; }
    ICourseCategoryRepository CourseCategories { get; }
    IRoleRepository Roles { get; }

    // দুইটা method রাখলাম
    Task<int> SaveChangesAsync();
    Task<int> CompleteAsync();

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
