namespace Inventory_Product.Application.Categories.DTOs
{
    public record CategoryResponse(int CategoryId,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
}