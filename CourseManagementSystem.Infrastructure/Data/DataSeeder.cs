using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Enums;
using CourseManagementSystem.Infrastructure.Services;

namespace CourseManagementSystem.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed Roles
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role { RoleName = "Admin", Description = "System Administrator" },
                new Role { RoleName = "Instructor", Description = "Course Instructor" },
                new Role { RoleName = "Student", Description = "Course Student" }
            };

            //await context.Roles.AddRangeAsync(roles);
            //await context.SaveChangesAsync();
        }

        // Seed Admin User
        if (!await context.Users.AnyAsync(u => u.Email == "admin"))
        {
            var adminRole = await context.Roles.FirstAsync(r => r.RoleName == "Admin");
            var passwordService = new PasswordService();

            var adminUser = new User
            {
                FullName = "System Administrator",
                Email = "admin@gmail.com",
                PhoneNumber = "1234567890",
                PasswordHash = passwordService.HashPassword("admin123"),
                RoleId = adminRole.Id,
                DateCreated = DateTime.UtcNow
            };

            //await context.Users.AddAsync(adminUser);
            //await context.SaveChangesAsync();
        }

        // Seed Course Categories
        if (!await context.CourseCategories.AnyAsync())
        {
            var categories = new List<CourseCategory>
            {
                new CourseCategory { Name = "Programming", Description = "Programming and software development courses" },
                new CourseCategory { Name = "Web Development", Description = "Web development and design courses" },
                new CourseCategory { Name = "Mobile Development", Description = "Mobile app development courses" },
                new CourseCategory { Name = "Data Science", Description = "Data science and analytics courses" },
                new CourseCategory { Name = "DevOps", Description = "DevOps and cloud computing courses" },
                new CourseCategory { Name = "Cybersecurity", Description = "Information security and cybersecurity courses" }
            };

            //await context.CourseCategories.AddRangeAsync(categories);
            //await context.SaveChangesAsync();
        }

        // Seed Sample Instructors
        if (!await context.Users.AnyAsync(u => u.Role.RoleName == "Instructor"))
        {
            var instructorRole = await context.Roles.FirstAsync(r => r.RoleName == "Instructor");
            var passwordService = new PasswordService();

            var instructors = new List<User>
            {
                new User
                {
                    FullName = "John Smith",
                    Email = "john.smith@example.com",
                    PhoneNumber = "1234567891",
                    PasswordHash = passwordService.HashPassword("instructor123"),
                    RoleId = instructorRole.Id,
                    DateCreated = DateTime.UtcNow
                },
                new User
                {
                    FullName = "Jane Doe",
                    Email = "jane.doe@example.com",
                    PhoneNumber = "1234567892",
                    PasswordHash = passwordService.HashPassword("instructor123"),
                    RoleId = instructorRole.Id,
                    DateCreated = DateTime.UtcNow
                }
            };

            //await context.Users.AddRangeAsync(instructors);
            //await context.SaveChangesAsync();
        }

        // Seed Sample Courses
        if (!await context.Courses.AnyAsync())
        {
            var programmingCategory = await context.CourseCategories.FirstAsync(c => c.Name == "Programming");
            var webDevCategory = await context.CourseCategories.FirstAsync(c => c.Name == "Web Development");
            var instructor1 = await context.Users.FirstAsync(u => u.Email == "john.smith@example.com");
            var instructor2 = await context.Users.FirstAsync(u => u.Email == "jane.doe@example.com");

            var courses = new List<Course>
            {
                new Course
                {
                    Name = "C# Fundamentals",
                    Description = "Learn the basics of C# programming language",
                    Price = 299.99m,
                    DiscountPrice = 199.99m,
                    CategoryId = programmingCategory.Id,
                    InstructorId = instructor1.Id,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                },
                new Course
                {
                    Name = "ASP.NET Core Web API",
                    Description = "Build modern web APIs with ASP.NET Core",
                    Price = 399.99m,
                    DiscountPrice = 299.99m,
                    CategoryId = webDevCategory.Id,
                    InstructorId = instructor2.Id,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                },
                new Course
                {
                    Name = "JavaScript for Beginners",
                    Description = "Learn JavaScript from scratch",
                    Price = 199.99m,
                    CategoryId = webDevCategory.Id,
                    InstructorId = instructor1.Id,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                }
            };

            //await context.Courses.AddRangeAsync(courses);
            //await context.SaveChangesAsync();
        }

        // Seed Sample Student
        if (!await context.Users.AnyAsync(u => u.Role.RoleName == "Student"))
        {
            var studentRole = await context.Roles.FirstAsync(r => r.RoleName == "Student");
            var passwordService = new PasswordService();

            var student = new User
            {
                FullName = "Alice Johnson",
                Email = "alice.johnson@example.com",
                PhoneNumber = "1234567893",
                PasswordHash = passwordService.HashPassword("student123"),
                RoleId = studentRole.Id,
                DateCreated = DateTime.UtcNow
            };

            await context.Users.AddAsync(student);
            await context.SaveChangesAsync();

            // Add sample enrollment
            var course = await context.Courses.FirstAsync();
            var enrollment = new Enrollment
            {
                CourseId = course.Id,
                UserId = student.Id,
                Description = "Sample enrollment for testing",
                PaymentAmount = course.DiscountPrice ?? course.Price,
                Discount = 0,
                EnrollmentDate = DateTime.UtcNow,
                PaymentDate = DateTime.UtcNow,
                EnrollmentStatus = EnrollmentStatus.Enrolled,
                PaymentStatus = PaymentStatus.Paid
            };

            //await context.Enrollments.AddAsync(enrollment);
            //await context.SaveChangesAsync();
        }
    }
}