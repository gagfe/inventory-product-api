namespace Inventory_Product.Application.Categories.DTOs
{
    public record UpdateCategoryRequest(
    string Name,
    string? Description);
}