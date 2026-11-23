namespace Inventory_Product.Application.Products.DTOs
{
    public record UpdateProductRequest(
        int CategoryId,
        string Sku,
        string Name,
        string? Description,
        decimal UnitPrice,
        int MinimumStock,
        byte[] RowVersion);
}