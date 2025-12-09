using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCOLLAPI.Repository;
using MyCOLLDB.Data;
using MyCOLLDB.Entities;
using MyCOLLDB.Entities.Constants;

namespace MyCOLLAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
	private readonly ProductRepository _repository;

	public ProductController(ApplicationDbContext dbcontext)
	{
		_repository = new ProductRepository(dbcontext);
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
	{
		var products = _repository.GetAllProducts();
		return Ok(products);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Product>> GetById(int id)
	{
		var product = _repository.GetProductById(id);
		if (product == null)
			return NotFound();

		return Ok(product);
	}

	[HttpPost]
	public async Task<ActionResult<Product>> CreateProduct(Product dto)
	{
		_repository.AddProduct(dto);
		return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateProduct(int id, Product dto)
	{
		if (id != dto.Id)
			return BadRequest();

		_repository.UpdateProduct(dto);
		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteProduct(int id)
	{
		_repository.DeleteProduct(id);
		return NoContent();
	}
}
