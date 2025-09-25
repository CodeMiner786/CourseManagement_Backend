using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CourseManagementSystem.Core.DTOs.CourseCategory;
using CourseManagementSystem.Core.Entities;
using CourseManagementSystem.Core.Interfaces;

namespace CourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseCategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CourseCategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all course categories
    /// </summary>
    /// <returns>List of course categories</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseCategoryDto>>> GetCourseCategories()
    {
        var categories = await _unitOfWork.CourseCategories.GetCategoriesWithCoursesAsync();
        var categoryDtos = _mapper.Map<IEnumerable<CourseCategoryDto>>(categories);
        return Ok(categoryDtos);
    }

    /// <summary>
    /// Get course category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Course category details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseCategoryDto>> GetCourseCategory(int id)
    {
        var category = await _unitOfWork.CourseCategories.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        var categoryDto = _mapper.Map<CourseCategoryDto>(category);
        return Ok(categoryDto);
    }

    /// <summary>
    /// Create a new course category
    /// </summary>
    /// <param name="createCategoryDto">Category creation details</param>
    /// <returns>Created course category</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CourseCategoryDto>> CreateCourseCategory([FromBody] CreateCourseCategoryDto createCategoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if category with same name already exists
        var existingCategory = await _unitOfWork.CourseCategories.GetByNameAsync(createCategoryDto.Name);
        if (existingCategory != null)
        {
            return BadRequest(new { message = "Category with this name already exists" });
        }

        var category = _mapper.Map<CourseCategory>(createCategoryDto);
        await _unitOfWork.CourseCategories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        var categoryDto = _mapper.Map<CourseCategoryDto>(category);
        return CreatedAtAction(nameof(GetCourseCategory), new { id = category.Id }, categoryDto);
    }

    /// <summary>
    /// Update course category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="updateCategoryDto">Category update details</param>
    /// <returns>Updated course category</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CourseCategoryDto>> UpdateCourseCategory(int id, [FromBody] UpdateCourseCategoryDto updateCategoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var category = await _unitOfWork.CourseCategories.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        // Check for duplicate name only if Name is being updated
        if (!string.IsNullOrWhiteSpace(updateCategoryDto.Name) && category.Name != updateCategoryDto.Name)
        {
            var existingCategory = await _unitOfWork.CourseCategories.GetByNameAsync(updateCategoryDto.Name);
            if (existingCategory != null)
            {
                return BadRequest(new { message = "Category with this name already exists" });
            }
        }

        // Map only provided fields (ignoring nulls)
        _mapper.Map(updateCategoryDto, category);

        _unitOfWork.CourseCategories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        var categoryDto = _mapper.Map<CourseCategoryDto>(category);
        return Ok(categoryDto);
    }


    /// <summary>
    /// Delete course category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCourseCategory(int id)
    {
        var category = await _unitOfWork.CourseCategories.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        // Check if category has courses
        var courses = await _unitOfWork.Courses.GetCoursesByCategoryAsync(id);
        if (courses.Any())
        {
            return BadRequest(new { message = "Cannot delete category that has courses. Please move or delete courses first." });
        }

        _unitOfWork.CourseCategories.Remove(category);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}