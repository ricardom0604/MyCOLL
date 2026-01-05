using System.ComponentModel.DataAnnotations;

namespace MyCOLL.Shared.Models.Dto;

public class RegisterDto
{
	[Required(ErrorMessage = "Full name is required.")]
	[StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
	public string FullName { get; set; } = string.Empty;

	[Required(ErrorMessage = "Email is required.")]
	[EmailAddress(ErrorMessage = "Enter a valid email.")]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "Password is required.")]
	[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = string.Empty;

	[Required(ErrorMessage = "Confirm password is required.")]
	[Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
	[DataType(DataType.Password)]
	public string ConfirmPassword { get; set; } = string.Empty;

	// Optional/required based on business rules
	public string? Nif { get; set; }
	public string? Address { get; set; }

	// Frontend sends "Client" or "Supplier"
	[Required(ErrorMessage = "Account type is required.")]
	public string Role { get; set; } = string.Empty;
}
