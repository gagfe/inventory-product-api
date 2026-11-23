using Inventory_Product.Application.InventoryMovements.DTOs;
namespace Inventory_Product.Application.Abstraction.Persistence
{
    public interface IInventoryMovementReadRepository
    {
        Task<IReadOnlyList<InventoryMovementResponse>> GetByProductIdAsync(int productId, CancellationToken cancellationToken);
    }
}