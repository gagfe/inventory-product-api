using Inventory_Product.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Inventory_Product.Repository.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.ProductId);
            builder.Property(p => p.ProductId).ValueGeneratedOnAdd();
            builder.Property(p => p.Sku).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Name).HasMaxLength(150).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(1000).IsRequired(false);
            builder.Property(p => p.UnitPrice).HasPrecision(18, 2).IsRequired();
            builder.Property(p => p.CurrentStock).IsRequired().HasDefaultValue(0);
            builder.Property(p => p.MinimumStock).IsRequired().HasDefaultValue(0);
            builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(p => p.UpdatedAt).HasColumnType("datetime2(0)").IsRequired(false);
            builder.Property(p => p.RowVersion).IsRowVersion();
            builder.HasIndex(p => p.Sku).IsUnique();
            builder.HasIndex(p => p.CategoryId).IncludeProperties(p => new { p.Name, p.Sku, p.UnitPrice, p.CurrentStock });
            builder.HasOne<Category>().WithMany().HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}