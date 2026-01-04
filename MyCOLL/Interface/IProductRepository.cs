using MyCOLL.Data.Models.Entities;

namespace MyCOLL.Interface;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetClientProducts();
    Task<IEnumerable<Product>> GetSupplierProducts(string userId);
    Task<Product?> GetProductById(int id);
    Task AddProduct(Product product);
    Task UpdateProduct(Product product);
    Task<ProductImage?> GetImageById(int id);
    Task DeleteImage(int imageId);
    Task DeleteProduct(int id);
}