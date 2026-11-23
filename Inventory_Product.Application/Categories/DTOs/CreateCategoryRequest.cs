namespace Inventory_Product.Application.Categories.DTOs
{
    public record CreateCategoryRequest(
    string Name,
    string? Description);
}