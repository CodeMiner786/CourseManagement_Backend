using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CourseManagementSystem.Core.DTOs.Enrollment;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Interfaces;

namespace CourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EnrollmentsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all enrollments
    /// </summary>
    /// <returns>List of enrollments</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollments()
    {
        var enrollments = await _unitOfWork.Enrollments.GetEnrollmentsWithDetailsAsync();
        var enrollmentDtos = _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
        return Ok(enrollmentDtos);
    }

    /// <summary>
    /// Get enrollment by ID
    /// </summary>
    /// <param name="id">Enrollment ID</param>
    /// <returns>Enrollment details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollment(int id)
    {
        var enrollment = await _unitOfWork.Enrollments.GetEnrollmentWithDetailsAsync(id);

        if (enrollment == null)
        {
            return NotFound();
        }

        // Users can only view their own enrollments unless they're admin or instructor of the course
        var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        if (!User.IsInRole("Admin") &&
            enrollment.UserId != currentUserId &&
            (!User.IsInRole("Instructor") || enrollment.Course.InstructorId != currentUserId))
        {
            return Forbid();
        }

        var enrollmentDto = _mapper.Map<EnrollmentDto>(enrollment);
        return Ok(enrollmentDto);
    }

    /// <summary>
    /// Create a new enrollment
    /// </summary>
    /// <param name="createEnrollmentDto">Enrollment creation details</param>
    /// <returns>Created enrollment</returns>
    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> CreateEnrollment([FromBody] CreateEnrollmentDto createEnrollmentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verify course exists
        var course = await _unitOfWork.Courses.GetByIdAsync(createEnrollmentDto.CourseId);
        if (course == null)
        {
            return BadRequest(new { message = "Invalid course" });
        }

        // Verify user exists
        var user = await _unitOfWork.Users.GetByIdAsync(createEnrollmentDto.UserId);
        if (user == null)
        {
            return BadRequest(new { message = "Invalid user" });
        }

        // Students can only enroll themselves
        var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        if (User.IsInRole("Student") && createEnrollmentDto.UserId != currentUserId)
        {
            return Forbid("Students can only enroll themselves");
        }

        // Check if user is already enrolled in the course
        var existingEnrollment = await _unitOfWork.Enrollments.IsUserEnrolledInCourseAsync(
            createEnrollmentDto.UserId, createEnrollmentDto.CourseId);

        if (existingEnrollment)
        {
            return BadRequest(new { message = "User is already enrolled in this course" });
        }

        var enrollment = _mapper.Map<Enrollment>(createEnrollmentDto);
        enrollment.EnrollmentDate = DateTime.UtcNow;

        await _unitOfWork.Enrollments.AddAsync(enrollment);
        await _unitOfWork.SaveChangesAsync();

        var createdEnrollment = await _unitOfWork.Enrollments.GetEnrollmentWithDetailsAsync(enrollment.Id);
        var enrollmentDto = _mapper.Map<EnrollmentDto>(createdEnrollment);

        return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollmentDto);
    }

    /// <summary>
    /// Update enrollment
    /// </summary>
    /// <param name="id">Enrollment ID</param>
    /// <param name="updateEnrollmentDto">Enrollment update details</param>
    /// <returns>Updated enrollment</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<EnrollmentDto>> UpdateEnrollment(int id, [FromBody] UpdateEnrollmentDto updateEnrollmentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var enrollment = await _unitOfWork.Enrollments.GetEnrollmentWithDetailsAsync(id);
        if (enrollment == null)
        {
            return NotFound();
        }

        // Instructors can only update enrollments for their courses
        if (User.IsInRole("Instructor"))
        {
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (enrollment.Course.InstructorId != currentUserId)
            {
                return Forbid("You can only update enrollments for your courses");
            }
        }

        _mapper.Map(updateEnrollmentDto, enrollment);
        _unitOfWork.Enrollments.Update(enrollment);
        await _unitOfWork.SaveChangesAsync();

        var updatedEnrollment = await _unitOfWork.Enrollments.GetEnrollmentWithDetailsAsync(enrollment.Id);
        var enrollmentDto = _mapper.Map<EnrollmentDto>(updatedEnrollment);

        return Ok(enrollmentDto);
    }

    /// <summary>
    /// Delete enrollment
    /// </summary>
    /// <param name="id">Enrollment ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(id);
        if (enrollment == null)
        {
            return NotFound();
        }

        _unitOfWork.Enrollments.Remove(enrollment);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Get enrollments by user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of enrollments for specified user</returns>
    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByUser(int userId)
    {
        // Users can only view their own enrollments unless they're admin
        var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        if (!User.IsInRole("Admin") && userId != currentUserId)
        {
            return Forbid();
        }

        var enrollments = await _unitOfWork.Enrollments.GetEnrollmentsByUserAsync(userId);
        var enrollmentDtos = _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
        return Ok(enrollmentDtos);
    }

    /// <summary>
    /// Get enrollments by course
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of enrollments for specified course</returns>
    [HttpGet("by-course/{courseId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByCourse(int courseId)
    {
        // Instructors can only view enrollments for their courses
        if (User.IsInRole("Instructor"))
        {
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);

            if (course == null)
            {
                return NotFound();
            }

            if (course.InstructorId != currentUserId)
            {
                return Forbid("You can only view enrollments for your courses");
            }
        }

        var enrollments = await _unitOfWork.Enrollments.GetEnrollmentsByCourseAsync(courseId);
        var enrollmentDtos = _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
        return Ok(enrollmentDtos);
    }

    /// <summary>
    /// Get current user's enrollments
    /// </summary>
    /// <returns>List of enrollments for current user</returns>
    [HttpGet("my-enrollments")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetMyEnrollments()
    {
        var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        var enrollments = await _unitOfWork.Enrollments.GetEnrollmentsByUserAsync(currentUserId);
        var enrollmentDtos = _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
        return Ok(enrollmentDtos);
    }
}