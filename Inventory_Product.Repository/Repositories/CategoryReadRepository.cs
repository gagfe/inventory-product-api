using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Categories.DTOs;
using Inventory_Product.Repository.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Product.Repository.Repositories
{
    public class CategoryReadRepository : ICategoryReadRepository
    {
        private readonly AppDbContext _context;

        public CategoryReadRepository(AppDbContext context)
            => _context = context;

        public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(category => category.IsActive)
                .OrderBy(category => category.Name)
                .Select(category => new CategoryResponse(
                    category.CategoryId,
                    category.Name,
                    category.Description,
                    category.IsActive,
                    category.CreatedAt,
                    category.UpdatedAt))
                .ToListAsync(cancellationToken);
        }

        public async Task<CategoryResponse?> GetByIdAsync(
            int categoryId,
            CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(category =>
                    category.CategoryId == categoryId &&
                    category.IsActive)
                .Select(category => new CategoryResponse(
                    category.CategoryId,
                    category.Name,
                    category.Description,
                    category.IsActive,
                    category.CreatedAt,
                    category.UpdatedAt))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}