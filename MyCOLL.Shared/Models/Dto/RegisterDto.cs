using System.ComponentModel.DataAnnotations;

namespace MyCOLL.Shared.Models.Dto;
public class RegisterDto
{
    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Introduza um email válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A password é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A password deve ter pelo menos 6 caracteres.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação da password é obrigatória.")]
    [Compare(nameof(Password), ErrorMessage = "As passwords não coincidem.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    // Campos opcionais ou obrigatórios dependendo da regra de negócio
    public string? Nif { get; set; }
    public string? Address { get; set; }

    // Importante: O Frontend vai enviar "Cliente" ou "Fornecedor"
    [Required(ErrorMessage = "O tipo de conta é obrigatório.")]
    public string Role { get; set; } = string.Empty;
}