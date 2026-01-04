using MyCOLL.Data.Models.Entities;

namespace MyCOLL.Interface;

public interface IOrderItemRepository
{
    Task<IEnumerable<OrderItem>> GetOrderItemsByOrderId(int orderId);
    Task<OrderItem?> GetOrderItemById(int id);
    Task AddOrderItem(OrderItem orderItem);
    Task UpdateOrderItem(OrderItem orderItem);
    Task DeleteOrderItem(int id);
}