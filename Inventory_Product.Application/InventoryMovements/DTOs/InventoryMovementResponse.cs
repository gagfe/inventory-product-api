namespace Inventory_Product.Application.InventoryMovements.DTOs
{
    public record InventoryMovementResponse(
    long MovementId,
    int ProductId,
    string ProductName,
    byte MovementTypeId,
    string MovementType,
    int Quantity,
    string? Reason,
    string CreatedBy,
    DateTime CreatedAt);
}