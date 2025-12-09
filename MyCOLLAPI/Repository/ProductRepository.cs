namespace MyCOLLAPI.Repository;

using MyCOLLDB.Data;
using MyCOLLDB.Entities;

public class ProductRepository
{
	private readonly ApplicationDbContext _context;
	public ProductRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public IEnumerable<Product> GetAllProducts()
	{
		return _context.Products.ToList();
	}

	public Product? GetProductById(int id)
	{
		return _context.Products.Find(id);
	}

	public void AddProduct(Product product)
	{
		_context.Products.Add(product);
		_context.SaveChanges();
	}

	public void UpdateProduct(Product product)
	{
		_context.Products.Update(product);
		_context.SaveChanges();
	}

	public void DeleteProduct(int id)
	{
		var product = _context.Products.Find(id);
		if (product == null)
			return;

		_context.Products.Remove(product);
		_context.SaveChanges();
	}
}
