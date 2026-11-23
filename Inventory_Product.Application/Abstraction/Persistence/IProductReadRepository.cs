using Inventory_Product.Application.Products.DTOs;
namespace Inventory_Product.Application.Abstraction.Persistence
{
    public interface IProductReadRepository
    {
        Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken);
    }
}