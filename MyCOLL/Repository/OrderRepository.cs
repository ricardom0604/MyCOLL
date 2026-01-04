using Microsoft.EntityFrameworkCore;
using MyCOLL.Data.Data;
using MyCOLL.Data.Models.Entities;
using MyCOLL.Interface;
using MyCOLL.Shared.Models.Dto;

namespace MyCOLL.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;
    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Order>> GetAllOrders()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
    public async Task<Order?> GetOrderById(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Product?> GetProductById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product;
    }
    public async Task AddOrder(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
}