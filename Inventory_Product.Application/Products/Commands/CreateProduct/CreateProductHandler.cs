using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Products.DTOs;
namespace Inventory_Product.Application.Products.Commands.CreateProduct
{
    public class CreateProductHandler
    {
        private readonly IProductCommandRepository _repository;
        public CreateProductHandler(IProductCommandRepository repository) => _repository = repository;
        public async Task<int?> HandleAsync(CreateProductRequest request, CancellationToken cancellationToken)
        {
            Validate(request);
            return await _repository.CreateAsync(request, cancellationToken);
        }
        private static void Validate(CreateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Sku);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);
            if (request.CategoryId <= 0 || request.Sku.Length > 50 || request.Name.Length > 150 ||
                request.Description?.Length > 1000 || request.UnitPrice < 0 ||
                request.UnitPrice > 9999999999999999.99m || request.MinimumStock < 0)
            {
                throw new ArgumentException("The product data is invalid.");
            }
        }
    }
}
