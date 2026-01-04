using Microsoft.EntityFrameworkCore;
using MyCOLL.Data.Models.Entities;
using MyCOLL.Interface;

namespace MyCOLL.Repository;

using MyCOLL.Data.Data;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await _context.Categories.ToListAsync();
    }
    
    public async Task<Category?> GetCategoryById(int id)
    {
        return await _context.Categories.FindAsync(id);
    }
    
    public async Task AddCategory(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCategory(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}