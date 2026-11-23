
using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.InventoryMovements.Commands.RegisterInventoryMovement;
using Inventory_Product.Application.InventoryMovements.DTOs;

namespace Inventory_Product.Test.InventoryMovements
{
    public class RegisterInventoryMovementHandlerTests
    {
        [Fact]
        public async Task HandleAsync_ShouldReturnSuccess_WhenMovementIsRegistered()
        {
            var repository = new FakeInventoryMovementCommandRepository(RegisterInventoryMovementResult.Success);
            var handler = new RegisterInventoryMovementHandler(repository);
            var request = CreateValidRequest();
            var result = await handler.HandleAsync(request, "inventory.admin", CancellationToken.None);
            Assert.Equal(RegisterInventoryMovementResult.Success, result);
            Assert.NotNull(repository.ReceivedRequest);
            Assert.Equal("inventory.admin", repository.ReceivedCreatedBy);
        }

        [Theory]
        [InlineData(RegisterInventoryMovementResult.ProductNotFound)]
        [InlineData(RegisterInventoryMovementResult.MovementTypeNotFound)]
        [InlineData(RegisterInventoryMovementResult.InsufficientStock)]
        public async Task HandleAsync_ShouldReturnRepositoryResult_WhenMovementCannotBeRegistered(
            RegisterInventoryMovementResult expectedResult)
        {
            var repository = new FakeInventoryMovementCommandRepository(expectedResult);
            var handler = new RegisterInventoryMovementHandler(repository);
            var result = await handler.HandleAsync(
                CreateValidRequest(),
                "inventory.admin",
                CancellationToken.None);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            var repository = new FakeInventoryMovementCommandRepository(RegisterInventoryMovementResult.Success);
            var handler = new RegisterInventoryMovementHandler(repository);
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => handler.HandleAsync(null!, "inventory.admin", CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenCreatedByIsEmpty()
        {
            var repository = new FakeInventoryMovementCommandRepository(RegisterInventoryMovementResult.Success);
            var handler = new RegisterInventoryMovementHandler(repository);
            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.HandleAsync(CreateValidRequest(), "", CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenProductIdIsInvalid()
        {
            var repository = new FakeInventoryMovementCommandRepository(RegisterInventoryMovementResult.Success);
            var handler = new RegisterInventoryMovementHandler(repository);
            var request = CreateValidRequest() with { ProductId = 0 };
            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.HandleAsync(request, "inventory.admin", CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenMovementTypeIdIsInvalid()
        {
            var repository = new FakeInventoryMovementCommandRepository(RegisterInventoryMovementResult.Success);
            var handler = new RegisterInventoryMovementHandler(repository);
            var request = CreateValidRequest() with { MovementTypeId = 0 };
            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.HandleAsync(request, "inventory.admin", CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenQuantityIsInvalid()
        {
            var repository = new FakeInventoryMovementCommandRepository(RegisterInventoryMovementResult.Success);
            var handler = new RegisterInventoryMovementHandler(repository);
            var request = CreateValidRequest() with { Quantity = 0 };
            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.HandleAsync(request, "inventory.admin", CancellationToken.None));
        }

        private static RegisterInventoryMovementRequest CreateValidRequest() =>
            new(1, 1, 10, "Stock replenishment");

        private sealed class FakeInventoryMovementCommandRepository : IInventoryMovementCommandRepository
        {
            private readonly RegisterInventoryMovementResult _result;

            public RegisterInventoryMovementRequest? ReceivedRequest { get; private set; }
            public string? ReceivedCreatedBy { get; private set; }

            public FakeInventoryMovementCommandRepository(RegisterInventoryMovementResult result)
            {
                _result = result;
            }

            public Task<RegisterInventoryMovementResult> RegisterAsync(
                RegisterInventoryMovementRequest request,
                string createdBy,
                CancellationToken cancellationToken)
            {
                ReceivedRequest = request;
                ReceivedCreatedBy = createdBy;
                return Task.FromResult(_result);
            }
        }
    }
}
