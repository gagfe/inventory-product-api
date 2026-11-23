using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Products.DTOs;
namespace Inventory_Product.Application.Products.Queries.GetProducts
{
    public class GetProductsHandler
    {
        private readonly IProductReadRepository _repository;
        public GetProductsHandler(IProductReadRepository repository) => _repository = repository;
        public Task<IReadOnlyList<ProductResponse>> HandleAsync(CancellationToken cancellationToken) => _repository.GetAllAsync(cancellationToken);
    }
}