using MyCOLL.Interface;
using MyCOLL.Repository;
using MyCOLL.Shared.Models.Dto;

namespace MyCOLL.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCOLL.Data.Data;
using MyCOLL.Data.Models.Entities;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _repository;
    
    public CategoryController(ICategoryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _repository.GetAllCategories();
        // Map Category entities to CategoryDtos
        var categoryDtos = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();
        return Ok(categoryDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetById(int id)
    {
        var category = await _repository.GetCategoryById(id);
        if (category == null)
            return NotFound();
        
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category dto)
    {
        await _repository.AddCategory(dto);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, Category dto)
    {
        if (id != dto.Id)
            return BadRequest();
        
        await _repository.UpdateCategory(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _repository.DeleteCategory(id);
        return NoContent();
    }
}