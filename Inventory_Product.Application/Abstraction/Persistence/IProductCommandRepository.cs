using Inventory_Product.Application.Products.Commands.UpdateProduct;
using Inventory_Product.Application.Products.DTOs;
namespace Inventory_Product.Application.Abstraction.Persistence
{
    public interface IProductCommandRepository
    {
        Task<int?> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
        Task<UpdateProductResult> UpdateAsync(int productId, UpdateProductRequest request, CancellationToken cancellationToken);
        Task<bool> DeactivateAsync(int productId, CancellationToken cancellationToken);
    }
}