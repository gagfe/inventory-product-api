namespace Inventory_Product.Application.Products.Commands.UpdateProduct
{
    public enum UpdateProductResult
    {
        Updated,
        NotFound,
        InvalidCategory,
        ConcurrencyConflict
    }
}