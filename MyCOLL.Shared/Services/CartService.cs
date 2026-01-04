using MyCOLL.Shared.Models.Dto;

namespace MyCOLL.Shared.Services;

public class CartService
{
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    public event Action? OnChange;

    public void AddToCart(ProductDto product)
    {
        var existingItem = Items.FirstOrDefault(i => i.Product.Id == product.Id);
        
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            Items.Add(new CartItem { Product = product, Quantity = 1 });
        }
        
        NotifyStateChanged();
    }

    public void RemoveFromCart(ProductDto product)
    {
        var item = Items.FirstOrDefault(i => i.Product.Id == product.Id);
        if (item != null)
        {
            Items.Remove(item);
            NotifyStateChanged();
        }
    }
    public void Clear()
    {
        Items.Clear();
        NotifyStateChanged();
    }

    public decimal TotalAmount() => Items.Sum(i => i.TotalPrice);

    private void NotifyStateChanged() => OnChange?.Invoke();
}