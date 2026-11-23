namespace Inventory_Product.Application.Products.DTOs
{
    public record ProductResponse(
        int ProductId,
        string Sku,
        string Name,
        string? Description,
        decimal UnitPrice,
        int CurrentStock,
        int MinimumStock,
        bool IsActive,
        byte[] RowVersion);
}