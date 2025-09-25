using CourseManagementSystem.Core.Entities;

namespace CourseManagementSystem.Core.Interfaces;

public interface ICourseCategoryRepository : IRepository<CourseCategory>
{
    Task<CourseCategory?> GetByNameAsync(string name);
    Task<IEnumerable<CourseCategory>> GetCategoriesWithCoursesAsync();
}