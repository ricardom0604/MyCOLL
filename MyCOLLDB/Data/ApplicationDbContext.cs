using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCOLLDB.Model.Entities;

namespace MyCOLLDB.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
	: IdentityDbContext<ApplicationUser>(options)
{
	public DbSet<Product> Products { get; set; }
	public DbSet<Category> Categories { get; set; }

	public DbSet<ApplicationUser> Suppliers { get; set; }
	public DbSet<ApplicationUser> Clients { get; set; }

	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }
	public DbSet<ProductImage> ProductImages { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// OBRIGATÓRIO: Configura as tabelas do Identity (Users, Roles, etc.)
		base.OnModelCreating(modelBuilder);

		// --- Configuração Global de Precisão para Dinheiro ---
		// Evita o erro: "Decimal property 'Price' ... no type specified"
		foreach (var property in modelBuilder.Model.GetEntityTypes()
			.SelectMany(t => t.GetProperties())
			.Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
		{
			property.SetPrecision(18);
			property.SetScale(2);
		}

		// --- Configurações da Entidade Product ---
		modelBuilder.Entity<Product>(entity =>
		{
			// Relação com Fornecedor (User)
			entity.HasOne(p => p.Supplier)
				.WithMany(u => u.Products)
				.HasForeignKey(p => p.SupplierId)
				.OnDelete(DeleteBehavior.Restrict); // Se apagar o User, não apaga os produtos (segurança)

			// Relação com Categoria
			entity.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.CategoryId);
		});

		// --- Configurações da Entidade Category (Hierarquia) ---
		modelBuilder.Entity<Category>(entity =>
		{
			// Auto-relacionamento (Categoria Pai -> Subcategorias)
			entity.HasOne(c => c.ParentCategory)
				.WithMany(c => c.SubCategories)
				.HasForeignKey(c => c.ParentCategoryId)
				.OnDelete(DeleteBehavior.Restrict); // Evita apagar uma categoria que tem filhas
		});

		// --- Configurações da Entidade Order (Encomendas) ---
		modelBuilder.Entity<Order>(entity =>
		{
			// Relação com Cliente (User)
			entity.HasOne(o => o.Client)
				.WithMany(u => u.Orders)
				.HasForeignKey(o => o.ClientId)
				.OnDelete(DeleteBehavior.Restrict);
		});

		// --- Configurações da Entidade OrderItem ---
		modelBuilder.Entity<OrderItem>(entity =>
		{
			// TRAVA DE SEGURANÇA CRÍTICA:
			// Impede que um produto seja apagado do banco se ele estiver numa encomenda.
			entity.HasOne(oi => oi.Product)
				.WithMany(p => p.OrderItems)
				.HasForeignKey(oi => oi.ProductId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}
}
