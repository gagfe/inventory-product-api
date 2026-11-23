using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.InventoryMovements.DTOs;
namespace Inventory_Product.Application.InventoryMovements.Queries.GetProductMovements
{
    public class GetProductMovementsHandler
    {
        private readonly IInventoryMovementReadRepository _repository;
        public GetProductMovementsHandler(IInventoryMovementReadRepository repository) => _repository = repository;
        public Task<IReadOnlyList<InventoryMovementResponse>> HandleAsync(int productId, CancellationToken cancellationToken)
        {
            if (productId <= 0)
                throw new ArgumentException("The product ID must be greater than zero.");
            return _repository.GetByProductIdAsync(productId, cancellationToken);
        }
    }
}