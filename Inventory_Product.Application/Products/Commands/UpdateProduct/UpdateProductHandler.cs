
using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Products.DTOs;

namespace Inventory_Product.Application.Products.Commands.UpdateProduct
{
    public class UpdateProductHandler
    {
        private readonly IProductCommandRepository _repository;

        public UpdateProductHandler(IProductCommandRepository repository)
            => _repository = repository;

        public async Task<UpdateProductResult> HandleAsync(
            int productId,
            UpdateProductRequest request,
            CancellationToken cancellationToken)
        {
            Validate(productId, request);

            return await _repository.UpdateAsync(
                productId,
                request,
                cancellationToken);
        }

        private static void Validate(
            int productId,
            UpdateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Sku);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);

            if (productId <= 0 ||
                request.CategoryId <= 0 ||
                request.Sku.Length > 50 ||
                request.Name.Length > 150 ||
                request.Description?.Length > 1000 ||
                request.UnitPrice < 0 ||
                request.UnitPrice > 9999999999999999.99m ||
                request.MinimumStock < 0 ||
                request.RowVersion is null ||
                request.RowVersion.Length != 8)
            {
                throw new ArgumentException(
                    "The product data is invalid.");
            }
        }
    }
}