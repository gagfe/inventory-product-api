using Inventory_Product.Application.Abstraction.Persistence;
namespace Inventory_Product.Application.Products.Commands.DeactivateProduct
{
    public class DeactivateProductHandler
    {
        private readonly IProductCommandRepository _repository;
        public DeactivateProductHandler(IProductCommandRepository repository) => _repository = repository;
        public Task<bool> HandleAsync(int productId, CancellationToken cancellationToken)
        {
            if (productId <= 0)
                throw new ArgumentException("The product ID must be greater than zero.");
            return _repository.DeactivateAsync(productId, cancellationToken);
        }
    }
}