using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyCOLLDB.Model.Entities;

public enum ProductType
{
	Collectible,
	Supplement
}

public enum AvailabilityMode
{
	ListingOnly,
	Sale,
	Rent
}

public class Product
{
	[Key]
	public int Id { get; set; }

	[Required, StringLength(60)]
	public string Name { get; set; } = string.Empty;

	[Required, StringLength(500)]
	public string Description { get; set; } = string.Empty;

	[Required, Range(0, int.MaxValue)]
	public int Stock { get; set; }

	[Required]
	public ProductType ProductType { get; set; }

	[Required]
	public AvailabilityMode AvailabilityMode { get; set; }

	[Required]
	public bool IsUsed { get; set; } = true;

	[Required]
	public bool IsActive { get; set; } = false;

	[Required]
	public string SupplierId { get; set; } = string.Empty;

	[ForeignKey(nameof(SupplierId))]
	public ApplicationUser Supplier { get; set; } = null!;

	[Required, Precision(18, 2)]
	public decimal BasePrice { get; set; }

	[Required, Range(0, 100), Precision(5, 2)]
	public decimal SellTax { get; set; }

	[Required, Precision(18, 2)]
	public decimal FinalPrice => BasePrice + (BasePrice * SellTax / 100);

	[Required]
	public int CategoryId { get; set; }

	[ForeignKey(nameof(CategoryId))]
	public Category Category { get; set; } = null!;

	public ICollection<OrderItem>? OrderItems { get; set; }

	public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
