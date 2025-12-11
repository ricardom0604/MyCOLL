using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyCOLLDB.Model.Entities;

public class Order
{
	[Key]
	public int Id { get; set; }

	[Required]
	public DateTime OrderDate { get; set; } = DateTime.Now;

	[Required, Precision(18, 2)]
	public decimal TotalAmount { get; set; }

	[Required, StringLength(20)]
	public string Status { get; set; } = "Pending";

	[Required]
	public string ClientId { get; set; } = string.Empty;

	[ForeignKey(nameof(ClientId))]
	public ApplicationUser? Client { get; set; }

	public ICollection<OrderItem>? Items { get; set; }
}
