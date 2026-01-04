using MyCOLL.Data.Models.Entities;

namespace MyCOLL.Interface;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllOrders();
    Task<Order?> GetOrderById(int id);
    Task<Product?> GetProductById(int id);
    Task AddOrder(Order order);
    Task DeleteOrder(int id);
    Task UpdateOrder(Order order);
}