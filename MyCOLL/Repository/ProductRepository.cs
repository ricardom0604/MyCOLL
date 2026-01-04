using Microsoft.EntityFrameworkCore;
using MyCOLL.Interface;

namespace MyCOLL.Repository;

using MyCOLL.Data.Data;
using MyCOLL.Data.Models.Entities;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Product>> GetClientProducts()
    {
        return await _context.Products
            .Include(p => p.Category)      // Carrega Categoria
            .Include(p => p.Supplier)      // Carrega Fornecedor
            .Include(p => p.Images)        // Carrega Imagens
            .Where(p => p.IsActive)        // Filtra só ativos (opcional)
            .AsNoTracking()                // <--- IMPORTANTE: Melhora performance para leitura
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetSupplierProducts(string userId)
    {
        return await _context.Products
            .Where(p => p.SupplierId == userId)
            .Include(p => p.Images)
            .ToListAsync();
    }
    
    public async Task<Product?> GetProductById(int id)
    {
        return await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task AddProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateProduct(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task<ProductImage?> GetImageById(int id)
    {
        return await _context.ProductImages.FindAsync(id);
    }
    
    public async Task DeleteImage(int imageId)
    {
        var image = await _context.ProductImages.FindAsync(imageId);
        if (image == null)
            return;
        
        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return;
        
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}