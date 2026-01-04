using System.ComponentModel.DataAnnotations;

namespace MyCOLL.Data.Models.Entities;

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