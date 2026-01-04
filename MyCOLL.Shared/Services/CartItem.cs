using MyCOLL.Shared.Models.Dto;

namespace MyCOLL.Shared.Services;

public class CartItem
{
    public ProductDto Product { get; set; } = new ProductDto();
    public int Quantity { get; set; } = 1;
    
    public decimal TotalPrice => Quantity * Product.FinalPrice; 
}