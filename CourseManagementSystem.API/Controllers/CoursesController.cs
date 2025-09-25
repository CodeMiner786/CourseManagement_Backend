using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CourseManagementSystem.Core.DTOs.Course;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Interfaces;

namespace CourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CoursesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // ===================== GET ALL =====================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        var courses = await _unitOfWork.Courses.GetCoursesWithDetailsAsync();
        var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
        return Ok(courseDtos);
    }

    // ===================== GET BY ID =====================
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(int id)
    {
        var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);

        if (course == null)
            return NotFound(new { message = "Course not found" });

        var courseDto = _mapper.Map<CourseDto>(course);
        return Ok(courseDto);
    }

    // ===================== CREATE =====================
    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate category
        var category = await _unitOfWork.CourseCategories.GetByIdAsync(createCourseDto.CategoryId);
        if (category == null)
            return BadRequest(new { message = "Invalid category" });

        // Validate instructor
        if (createCourseDto.InstructorId.HasValue)
        {
            var instructor = await _unitOfWork.Users.GetByIdAsync(createCourseDto.InstructorId.Value);
            if (instructor == null)
                return BadRequest(new { message = "Invalid instructor" });

            var instructorWithRole = await _unitOfWork.Users.GetUserWithRoleAsync(createCourseDto.InstructorId.Value);
            if (instructorWithRole?.Role.RoleName != "Instructor")
                return BadRequest(new { message = "Specified user is not an instructor" });
        }

        // If current user is instructor → can only assign self
        if (User.IsInRole("Instructor"))
        {
            var currentUserId = int.TryParse(User.FindFirst("UserId")?.Value, out var uid) ? uid : 0;
            createCourseDto.InstructorId = currentUserId;
        }

        var course = _mapper.Map<Course>(createCourseDto);
        course.DateCreated = DateTime.UtcNow;
        course.DateUpdated = DateTime.UtcNow;

        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();

        var createdCourse = await _unitOfWork.Courses.GetCourseWithDetailsAsync(course.Id);
        var courseDto = _mapper.Map<CourseDto>(createdCourse);

        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, courseDto);
    }

    // ===================== UPDATE =====================
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);
        if (course == null)
            return NotFound(new { message = "Course not found" });

        // Instructor update / unassign
        if (dto.InstructorId.HasValue)
        {
            var instructor = await _unitOfWork.Users.GetByIdAsync(dto.InstructorId.Value);
            if (instructor == null)
                return BadRequest(new { message = "Instructor not found" });

            course.InstructorId = instructor.Id;
        }
        else
        {
            // Unassign instructor
            course.InstructorId = null;
        }

        // Map remaining fields
        _mapper.Map(dto, course);
        course.DateUpdated = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        var updated = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);
        var result = _mapper.Map<CourseDto>(updated);
        return Ok(result);
    }

    // ===================== DELETE =====================
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        _unitOfWork.Courses.Remove(course);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    // ===================== GET BY CATEGORY =====================
    [HttpGet("by-category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByCategory(int categoryId)
    {
        var courses = await _unitOfWork.Courses.GetCoursesByCategoryAsync(categoryId);
        var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
        return Ok(courseDtos);
    }

    // ===================== GET BY INSTRUCTOR =====================
    [HttpGet("by-instructor/{instructorId}")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByInstructor(int instructorId)
    {
        var courses = await _unitOfWork.Courses.GetCoursesByInstructorAsync(instructorId);
        var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
        return Ok(courseDtos);
    }

    // ===================== GET CURRENT INSTRUCTOR COURSES =====================
    [HttpGet("my-courses")]
    [Authorize(Roles = "Instructor")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetMyCourses()
    {
        var currentUserId = int.TryParse(User.FindFirst("UserId")?.Value, out var uid) ? uid : 0;
        var courses = await _unitOfWork.Courses.GetCoursesByInstructorAsync(currentUserId);
        var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
        return Ok(courseDtos);
    }
}
