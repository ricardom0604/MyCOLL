using System.ComponentModel.DataAnnotations;

namespace MyCOLLDB.Model.Entities;

public class Category
{
	[Key]
	public int Id { get; set; }

	[Required, StringLength(50)]
	public string Name { get; set; } = string.Empty;

	public int? ParentCategoryId { get; set; }
	public Category? ParentCategory { get; set; }
	public ICollection<Category>? SubCategories { get; set; }

	public ICollection<Product>? Products { get; set; }
}
