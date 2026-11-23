namespace Inventory_Product.Application.InventoryMovements.DTOs
{
    public enum RegisterInventoryMovementResult
    {
        Success,
        ProductNotFound,
        MovementTypeNotFound,
        InsufficientStock
    }
}