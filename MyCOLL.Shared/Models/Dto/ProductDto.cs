using MyCOLL.Shared.Models.Enums; // Adicione o using dos Enums

namespace MyCOLL.Shared.Models.Dto;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Stock { get; set; }
    
    public ProductType ProductType { get; set; }
    public AvailabilityMode AvailabilityMode { get; set; }
    
    public bool IsUsed { get; set; }
    public bool IsActive { get; set; }
    
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty; // Útil para mostrar quem vendeu
    
    public decimal BasePrice { get; set; }
    public decimal SellTax { get; set; }
    public decimal FinalPrice { get; set; } // Valor calculado vem pronto da API
    
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty; // Para evitar trazer o obj Category inteiro
    
    public List<String> NewImagesBase64 { get; set; } = new(); // Imagens novas em Base64 para upload
    // Lista de URLs ou DTOs de imagem
    public List<ProductImageDto> Images { get; set; } = new();
}

// DTO Auxiliar para Imagem (Simples)
public class ProductImageDto 
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    
}