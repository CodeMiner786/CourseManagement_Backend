using Microsoft.EntityFrameworkCore.Storage;
using CourseManagementSystem.Core.Interfaces;
using CourseManagementSystem.Infrastructure.Data;

namespace CourseManagementSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private ICourseRepository? _courses;
    private IEnrollmentRepository? _enrollments;
    private ICourseCategoryRepository? _courseCategories;
    private IRoleRepository? _roles;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public ICourseRepository Courses => _courses ??= new CourseRepository(_context);
    public IEnrollmentRepository Enrollments => _enrollments ??= new EnrollmentRepository(_context);
    public ICourseCategoryRepository CourseCategories => _courseCategories ??= new CourseCategoryRepository(_context);
    public IRoleRepository Roles => _roles ??= new RoleRepository(_context);

    // ✅ প্রথমটি
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // ✅ দ্বিতীয়টি (SaveChangesAsync কে alias করা হলো)
    public async Task<int> CompleteAsync()
    {
        return await SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
