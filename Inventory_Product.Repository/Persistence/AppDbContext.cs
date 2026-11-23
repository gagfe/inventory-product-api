using Inventory_Product.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Inventory_Product.Repository.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<MovementType> MovementTypes => Set<MovementType>();
        public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}