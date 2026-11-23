using Inventory_Product.Application.InventoryMovements.DTOs;
namespace Inventory_Product.Application.Abstraction.Persistence
{
    public interface IInventoryMovementCommandRepository
    {
        Task<RegisterInventoryMovementResult> RegisterAsync(RegisterInventoryMovementRequest request, string createdBy, CancellationToken cancellationToken);
    }
}