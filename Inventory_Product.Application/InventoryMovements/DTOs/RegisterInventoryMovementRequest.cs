namespace Inventory_Product.Application.InventoryMovements.DTOs
{
    public record RegisterInventoryMovementRequest(
    int ProductId,
    byte MovementTypeId,
    int Quantity,
    string? Reason);
}