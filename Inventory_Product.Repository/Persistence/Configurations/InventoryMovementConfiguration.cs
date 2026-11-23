using Inventory_Product.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Inventory_Product.Repository.Persistence.Configurations
{
    public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryMovement> builder)
        {
            builder.ToTable("InventoryMovements");
            builder.HasKey(m => m.MovementId);
            builder.Property(m => m.MovementId).ValueGeneratedOnAdd();
            builder.Property(m => m.Quantity).IsRequired();
            builder.Property(m => m.Reason).HasMaxLength(250).IsRequired(false);
            builder.Property(m => m.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(m => m.CreatedAt).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
            builder.HasOne<Product>().WithMany().HasForeignKey(m => m.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<MovementType>().WithMany().HasForeignKey(m => m.MovementTypeId).OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(m => new { m.ProductId, m.CreatedAt }).IsDescending(false, true);
            builder.HasIndex(m => m.MovementTypeId);
        }
    }
}