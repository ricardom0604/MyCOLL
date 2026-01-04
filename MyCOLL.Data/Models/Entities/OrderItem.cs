using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCOLL.Data.Models.Entities;

public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public Order? Order { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
}