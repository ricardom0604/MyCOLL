using System.ComponentModel.DataAnnotations;

namespace MyCOLLDB.Entities.Dto;

public class LoginDto
{
	[Required(ErrorMessage = "O email é obrigatório.")]
	[EmailAddress(ErrorMessage = "Formato de email inválido.")]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "A password é obrigatória.")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = string.Empty;
}
