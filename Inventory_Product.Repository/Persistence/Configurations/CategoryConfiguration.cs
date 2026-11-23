using Inventory_Product.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Inventory_Product.Repository.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(categories => categories.CategoryId);
            builder.Property(categories => categories.CategoryId).ValueGeneratedOnAdd();
            builder.Property(categories => categories.Name).HasMaxLength(100).IsRequired();
            builder.Property(categories => categories.Description).HasMaxLength(500).IsRequired();
            builder.Property(categories => categories.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(categories => categories.CreatedAt).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(categories => categories.UpdatedAt).HasColumnType("datetime2(0)").IsRequired(false);
            builder.HasIndex(categories => categories.Name).IsUnique();
        }
    }
}