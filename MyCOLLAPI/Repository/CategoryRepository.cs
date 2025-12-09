using MyCOLLDB.Entities;
using MyCOLLDB.Data;


namespace MyCOLLAPI.Repository;

public class CategoryRepository
{
	private readonly ApplicationDbContext _context;
	public CategoryRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public IEnumerable<Category> GetCategories()
	{
		return _context.Categories.ToList();
	}

	public Category? GetCategoryById(int id)
	{
		return _context.Categories.Find(id);
	}

	public void AddCategory(Category category)
	{
		_context.Categories.Add(category);
		_context.SaveChanges();
	}

	public void UpdateCategory(Category category)
	{
		_context.Categories.Update(category);
		_context.SaveChanges();
	}

	public void DeleteCategory(int id)
	{
		var category = _context.Categories.Find(id);
		if (category == null)
			return;

		_context.Categories.Remove(category);
		_context.SaveChanges();
	}
}
