using MyCOLL.Data.Models.Entities;

namespace MyCOLL.Interface;

public interface ICategoryRepository
{
    Task <IEnumerable<Category>> GetAllCategories();
    Task <Category?> GetCategoryById(int id);
    Task AddCategory(Category category);
    Task UpdateCategory(Category category);
    Task DeleteCategory(int id);
}