using Inventory_Product.Application.Categories.DTOs;
namespace Inventory_Product.Application.Abstraction.Persistence
{
    public interface ICategoryCommandRepository
    {
        Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken);
        Task<bool> DeactivateAsync(int categoryId, CancellationToken cancellationToken);
    }
}