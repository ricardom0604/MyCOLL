using System.ComponentModel.DataAnnotations;

namespace MyCOLLDB.Model.Entities;

public class ProductImage
{
	[Key]
	public int Id { get; set; }

	[Required, StringLength(200)]
	public string ImageUrl { get; set; } = string.Empty;

	[Required]
	public bool IsPrimary { get; set; } = false;

	[Required]
	public int ProductId { get; set; }
}
