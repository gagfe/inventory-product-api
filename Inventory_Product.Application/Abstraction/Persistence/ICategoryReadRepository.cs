using Inventory_Product.Application.Categories.DTOs;
namespace Inventory_Product.Application.Abstraction.Persistence
{
    public interface ICategoryReadRepository
    {
        Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<CategoryResponse?> GetByIdAsync(int categoryId, CancellationToken cancellationToken);
    }
}