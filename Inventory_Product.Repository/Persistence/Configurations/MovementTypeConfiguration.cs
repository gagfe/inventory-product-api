using Inventory_Product.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Inventory_Product.Repository.Persistence.Configurations
{
    public class MovementTypeConfiguration : IEntityTypeConfiguration<MovementType>
    {
        public void Configure(EntityTypeBuilder<MovementType> builder)
        {
            builder.ToTable("MovementTypes");
            builder.HasKey(m => m.MovementTypeId);
            builder.Property(m => m.MovementTypeId).ValueGeneratedNever();
            builder.Property(m => m.Name).HasMaxLength(30).IsRequired();
            builder.Property(m => m.StockSign).IsRequired();
            builder.HasIndex(m => m.Name).IsUnique();
        }
    }
}