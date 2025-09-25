using AutoMapper;
using CourseManagementSystem.Core.DTOs.Course;
using CourseManagementSystem.Core.DTOs.CourseCategory;
using CourseManagementSystem.Core.DTOs.Enrollment;
using CourseManagementSystem.Core.DTOs.User;
using CourseManagementSystem.Core.Entities;

namespace CourseManagementSystem.Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // =============================
        // User mappings
        // =============================
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.InstructorCourses, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore());

        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.InstructorCourses, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore())
            // ✅ NEW: Only update if non-null
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // =============================
        // Course mappings
        // =============================
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor != null ? src.Instructor.FullName : null));

        CreateMap<CreateCourseDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
            .ForMember(dest => dest.DateUpdated, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Instructor, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore());

        CreateMap<UpdateCourseDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
            .ForMember(dest => dest.DateUpdated, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Instructor, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore())
            .ForMember(dest => dest.InstructorId, opt => opt.Ignore())
            // ✅ NEW: Only update if non-null
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // =============================
        // Enrollment mappings
        // =============================
        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));

        CreateMap<CreateEnrollmentDto, Enrollment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentTotal, opt => opt.Ignore())
            .ForMember(dest => dest.EnrollmentDate, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<UpdateEnrollmentDto, Enrollment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentTotal, opt => opt.Ignore())
            .ForMember(dest => dest.EnrollmentDate, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            // ✅ NEW: Only update if non-null
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // =============================
        // CourseCategory mappings
        // =============================
        CreateMap<CourseCategory, CourseCategoryDto>()
            .ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.Courses.Count));

        CreateMap<CreateCourseCategoryDto, CourseCategory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Courses, opt => opt.Ignore());

        CreateMap<UpdateCourseCategoryDto, CourseCategory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Courses, opt => opt.Ignore())
            // ✅ NEW: Only update if non-null
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}