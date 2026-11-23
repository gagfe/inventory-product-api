using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.InventoryMovements.DTOs;
using Inventory_Product.Application.InventoryMovements.Queries.GetProductMovements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory_Product.Test.InventoryMovements
{
    public class GetProductMovementsHandlerTests
    {
        [Fact]
        public async Task HandleAsync_ShouldReturnMovements_WhenProductHasHistory()
        {
            var expectedMovements = new List<InventoryMovementResponse>
        {
            new(1, 1, "Wireless Mouse", 1, "Entry", 10, "Stock replenishment", "admin", DateTime.UtcNow),
            new(2, 1, "Wireless Mouse", 2, "Exit", 4, "Customer order", "admin", DateTime.UtcNow)
        };
            var repository = new FakeInventoryMovementReadRepository(expectedMovements);
            var handler = new GetProductMovementsHandler(repository);
            var result = await handler.HandleAsync(1, CancellationToken.None);
            Assert.Equal(2, result.Count);
            Assert.Equal(expectedMovements, result);
            Assert.Equal(1, repository.ReceivedProductId);
        }
        [Fact]
        public async Task HandleAsync_ShouldReturnEmptyList_WhenProductHasNoHistory()
        {
            var repository = new FakeInventoryMovementReadRepository(new List<InventoryMovementResponse>());
            var handler = new GetProductMovementsHandler(repository);
            var result = await handler.HandleAsync(1, CancellationToken.None);
            Assert.Empty(result);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenProductIdIsInvalid(
            int productId)
        {
            var repository = new FakeInventoryMovementReadRepository(new List<InventoryMovementResponse>());
            var handler = new GetProductMovementsHandler(repository);
            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(productId, CancellationToken.None));
        }
        private sealed class FakeInventoryMovementReadRepository : IInventoryMovementReadRepository
        {
            private readonly IReadOnlyList<InventoryMovementResponse> _movements;
            public int ReceivedProductId { get; private set; }
            public FakeInventoryMovementReadRepository(IReadOnlyList<InventoryMovementResponse> movements) => _movements = movements;
            public Task<IReadOnlyList<InventoryMovementResponse>> GetByProductIdAsync(int productId, CancellationToken cancellationToken)
            {
                ReceivedProductId = productId;
                return Task.FromResult(_movements);
            }
        }
    }
}