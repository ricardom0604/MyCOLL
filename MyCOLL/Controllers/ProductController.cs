using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using MyCOLL.Shared.Constants;
using MyCOLL.Data.Models.Entities;
using MyCOLL.Interface;
using MyCOLL.Shared.Models.Dto;

namespace MyCOLL.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _repository;    
    private readonly IWebHostEnvironment _environment;
    
    public ProductController(IProductRepository repository, IWebHostEnvironment environment)
    {
        _repository = repository;
        _environment = environment;
    }

    // GET: api/Product
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        // 1. Busca as Entidades do Banco de Dados
        var productsFromDb = await _repository.GetClientProducts();

        // 2. Transforma (Mapeia) Entidade -> DTO
        var productsDto = productsFromDb.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Stock = p.Stock,
            BasePrice = p.BasePrice,
            FinalPrice = p.FinalPrice, // Valor calculado na Entidade
            IsActive = p.IsActive,
            ProductType = p.ProductType,
            
            // Mapeamento de Objetos Relacionados (Evita NullReferenceException)
            CategoryId = p.CategoryId,
            CategoryName = p.Category != null ? p.Category.Name : "Sem Categoria",
            
            SupplierId = p.SupplierId,
            SupplierName = p.Supplier != null ? p.Supplier.UserName : "Desconhecido",

            // Mapeamento da Lista de Imagens
            Images = p.Images.Select(img => new ProductImageDto 
            { 
                Id = img.Id, 
                ImageUrl = img.ImageUrl 
            }).ToList()
        }).ToList();

        // 3. Retorna a lista de DTOs limpa
        return Ok(productsDto);
    }
    
    // GET: api/Product/sup
    [HttpGet("sup/{supId}")]
    [Authorize(Roles = UserRoles.Supplier)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetSupplierProducts(string supId)
    {
        var products = await _repository.GetSupplierProducts(supId);

        // Mapeamento manual
        var dtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            BasePrice = p.BasePrice,
            SellTax = p.SellTax,
            Stock = p.Stock,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
        
            Images = p.Images.Select(img => new ProductImageDto 
            { 
                Id = img.Id, 
                ImageUrl = img.ImageUrl,
            }).ToList() 
        }).ToList();

        return Ok(dtos);
    }

    // GET: api/Product/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var product = await _repository.GetProductById(id);
        if (product == null || product.SupplierId != userId)
            return NotFound();
        
        return Ok(product);
    }

    // POST: api/Product
    [HttpPost]
    [Authorize(Roles = UserRoles.Supplier)]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Forbid();
        
        dto.SupplierId = userId;
        
        // Mapear DTO para Entidade
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            SellTax = dto.SellTax,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            SupplierId = dto.SupplierId,
            ProductType = dto.ProductType,
            AvailabilityMode = dto.AvailabilityMode,
            IsUsed = dto.IsUsed,
            IsActive = dto.IsActive,
            Images = new List<ProductImage>()
        };
        // --- LÓGICA DE PROCESSAMENTO DE IMAGENS BASE64 ---
        if (dto.NewImagesBase64.Count != 0 && dto.NewImagesBase64.Any())
        {
            var uploadFolder = Path.Combine(_environment.WebRootPath, "product-images");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            foreach (var base64 in dto.NewImagesBase64)
            {
                try
                {
                    // O formato vem como "data:image/jpeg;base64,/9j/4AAQSk..."
                    // Precisamos separar o cabeçalho dos dados reais.
                    var parts = base64.Split(',');
                    if (parts.Length != 2) continue; // Formato inválido

                    var header = parts[0]; // ex: data:image/jpeg;base64
                    var data = parts[1];   // ex: /9j/4AAQSk...

                    // Descobrir a extensão baseada no cabeçalho
                    string extension = ".jpg"; // Padrão
                    if (header.Contains("image/png")) extension = ".png";
                    else if (header.Contains("image/jpeg")) extension = ".jpg";

                    // Gerar nome único
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadFolder, fileName);

                    // Converter Base64 de volta para bytes
                    byte[] imageBytes = Convert.FromBase64String(data);

                    // Salvar no disco do servidor
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    // Adicionar à entidade Produto
                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = $"/product-images/{fileName}", // Caminho relativo para BD
                        IsPrimary = !product.Images.Any()
                    });
                }
                catch (Exception ex)
                {
                    // Logar erro mas tentar continuar com as outras imagens
                    Console.WriteLine($"Erro ao processar imagem Base64: {ex.Message}");
                }
            }
        }
        await _repository.AddProduct(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT: api/Product/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto dto)
    {
        if (id != dto.Id)
            return BadRequest();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var product = await _repository.GetProductById(id);
        if (product == null || product.SupplierId != userId)
            return NotFound("Produto não encontrado ou acesso negado.");
        
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.BasePrice = dto.BasePrice;
        product.SellTax = dto.SellTax;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;
        product.ProductType = dto.ProductType;
        product.AvailabilityMode = dto.AvailabilityMode;
        product.IsUsed = dto.IsUsed;
        product.IsActive = dto.IsActive;
        
        if (dto.NewImagesBase64.Count > 0 && dto.NewImagesBase64.Any())
        {
            var uploadFolder = Path.Combine(_environment.WebRootPath, "product-images");
            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

            foreach (var base64 in dto.NewImagesBase64)
            {
                try
                {
                    // O formato vem como "data:image/jpeg;base64,/9j/4AAQSk..."
                    // Precisamos separar o cabeçalho dos dados reais.
                    var parts = base64.Split(',');
                    if (parts.Length != 2) continue; // Formato inválido

                    var header = parts[0]; // ex: data:image/jpeg;base64
                    var data = parts[1];   // ex: /9j/4AAQSk...

                    // Descobrir a extensão baseada no cabeçalho
                    var extension = ".jpg"; // Padrão
                    if (header.Contains("image/png")) extension = ".png";
                    else if (header.Contains("image/jpeg")) extension = ".jpg";

                    // Gerar nome único
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadFolder, fileName);

                    // Converter Base64 de volta para bytes
                    var imageBytes = Convert.FromBase64String(data);

                    // Salvar no disco do servidor
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    // Adicionar à entidade Produto
                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = $"/product-images/{fileName}", // Caminho relativo para BD
                        IsPrimary = !product.Images.Any()
                    });
                }
                catch (Exception ex)
                {
                    // Logar erro mas tentar continuar com as outras imagens
                    Console.WriteLine($"Erro ao processar imagem Base64: {ex.Message}");
                }
            }
        }
        
        await _repository.UpdateProduct(product);
        return NoContent();
    }
    
    // DELETE: api/Product/images/5
    [HttpDelete("images/{imageId}")]
    public async Task<IActionResult> DeleteProductImage(int imageId)
    {
        // Precisará adicionar lógica no repositório para apagar imagem por ID
        var image = await _repository.GetImageById(imageId);
        if (image == null) return NotFound();

        // 1. Apagar ficheiro do disco
        var path = Path.Combine(_environment.WebRootPath, image.ImageUrl.TrimStart('/'));
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

        // 2. Apagar da BD
        await _repository.DeleteImage(imageId);
    
        return NoContent();
    }

    // DELETE: api/Product/5
    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Supplier)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var product = await _repository.GetProductById(id);
        if (product == null || product.SupplierId != userId)
            return Forbid();
        await _repository.DeleteProduct(id);
        return NoContent();
    }
}