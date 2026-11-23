using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.InventoryMovements.DTOs;
namespace Inventory_Product.Application.InventoryMovements.Commands.RegisterInventoryMovement
{
    public class RegisterInventoryMovementHandler
    {
        private readonly IInventoryMovementCommandRepository _repository;
        public RegisterInventoryMovementHandler(IInventoryMovementCommandRepository repository) => _repository = repository;
        public Task<RegisterInventoryMovementResult> HandleAsync(
            RegisterInventoryMovementRequest request,
            string createdBy,
            CancellationToken cancellationToken)
        {
            Validate(request, createdBy);
            return _repository.RegisterAsync(request, createdBy, cancellationToken);
        }
        private static void Validate(RegisterInventoryMovementRequest request, string createdBy)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);
            if (request.ProductId <= 0 || request.MovementTypeId <= 0 || request.Quantity <= 0 || request.Reason?.Length > 250 || createdBy.Length > 100)
                throw new ArgumentException("The inventory movement data is invalid.");
        }
    }
}