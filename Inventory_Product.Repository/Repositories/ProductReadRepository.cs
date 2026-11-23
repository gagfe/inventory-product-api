using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Products.DTOs;
using Inventory_Product.Repository.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Product.Repository.Repositories
{
    public class ProductReadRepository : IProductReadRepository
    {
        private readonly AppDbContext _context;

        public ProductReadRepository(AppDbContext context)
            => _context = context;

        public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(product => product.IsActive)
                .OrderBy(product => product.Name)
                .Select(product => new ProductResponse(
                    product.ProductId,
                    product.Sku,
                    product.Name,
                    product.Description,
                    product.UnitPrice,
                    product.CurrentStock,
                    product.MinimumStock,
                    product.IsActive,
                    product.RowVersion))
                .ToListAsync(cancellationToken);
        }
    }
}