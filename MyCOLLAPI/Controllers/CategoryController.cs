using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCOLLAPI.Repository;
using MyCOLLDB.Data;
using MyCOLLDB.Entities;

namespace MyCOLLAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
	private readonly CategoryRepository _repository;

	public CategoryController(ApplicationDbContext dbcontext)
	{
		_repository = new CategoryRepository(dbcontext);
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
	{
		var categories = _repository.GetCategories();
		return Ok(categories);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Category>> GetById(int id)
	{
		var category = _repository.GetCategoryById(id);
		if (category == null)
			return NotFound();

		return Ok(category);
	}

	[HttpPost]
	public async Task<ActionResult<Category>> CreateCategory(Category dto)
	{
		_repository.AddCategory(dto);
		return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateCategory(int id, Category dto)
	{
		if (id != dto.Id)
			return BadRequest();

		_repository.UpdateCategory(dto);
		return NoContent();
	}

	[HttpDelete("{id}")]

	public async Task<IActionResult> DeleteCategory(int id)
	{
		_repository.DeleteCategory(id);
		return NoContent();
	}
}
