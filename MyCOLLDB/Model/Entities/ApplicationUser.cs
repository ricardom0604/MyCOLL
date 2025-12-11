using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MyCOLLDB.Model.Entities;

public enum StateAccount
{
	Active,
	Pending,
	Suspended
}

public class ApplicationUser : IdentityUser
{
	[StringLength(50)]
	public string FullName { get; set; } = string.Empty;

	[StringLength(9)]
	public string? Nif { get; set; }

	[StringLength(100)]
	public string? Address { get; set; }

	public StateAccount StateAccount { get; set; } = StateAccount.Pending;

	public ICollection<Order>? Orders { get; set; }
	public ICollection<Product>? Products { get; set; }
}
