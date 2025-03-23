using dotnet_api_erp.src.Domain.Entities.ProductContext;
using dotnet_api_erp.src.Domain.Entities.SalesContext;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api_erp.src.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // CONTEXTO DE USER
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        
        // CONTEXTO DE PRODUCT
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }

        // CONTEXTO DE SALES
        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Cpf).IsUnique();
            });
            modelBuilder.Entity<Client>(entity => {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Cpf).IsUnique();
            });
            modelBuilder.Entity<Supplier>(entity => {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.CNPJ).IsUnique();
            });
            
         base.OnModelCreating(modelBuilder);
        }
    }   
}