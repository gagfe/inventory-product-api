using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Products.Commands.UpdateProduct;
using Inventory_Product.Application.Products.DTOs;

namespace Inventory_Product.Test.Products
{
    public class UpdateProductHandlerTests
    {
        [Theory]
        [InlineData(UpdateProductResult.Updated)]
        [InlineData(UpdateProductResult.NotFound)]
        [InlineData(UpdateProductResult.InvalidCategory)]
        [InlineData(UpdateProductResult.ConcurrencyConflict)]
        public async Task HandleAsync_ShouldReturnRepositoryResult(UpdateProductResult expectedResult)
        {
            var repository = new FakeProductCommandRepository(expectedResult);
            var handler = new UpdateProductHandler(repository);
            var request = CreateValidRequest();
            var result = await handler.HandleAsync(1, request, CancellationToken.None);
            Assert.Equal(expectedResult, result);
            Assert.Equal(1, repository.ReceivedProductId);
            Assert.Same(request, repository.ReceivedUpdateRequest);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            var repository = new FakeProductCommandRepository(UpdateProductResult.Updated);
            var handler = new UpdateProductHandler(repository);
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.HandleAsync(1, null!, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenProductIdIsInvalid()
        {
            var repository = new FakeProductCommandRepository(UpdateProductResult.Updated);
            var handler = new UpdateProductHandler(repository);
            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(0, CreateValidRequest(),CancellationToken.None));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenSkuIsInvalid(string sku)
        {
            var repository = new FakeProductCommandRepository(UpdateProductResult.Updated);
            var handler = new UpdateProductHandler(repository);
            var request = CreateValidRequest() with { Sku = sku };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(1, request, CancellationToken.None));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenNameIsInvalid(string name)
        {
            var repository = new FakeProductCommandRepository(UpdateProductResult.Updated);
            var handler = new UpdateProductHandler(repository);
            var request = CreateValidRequest() with { Name = name };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(1, request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenCategoryIdIsInvalid()
        {
            var repository = new FakeProductCommandRepository(UpdateProductResult.Updated);
            var handler = new UpdateProductHandler(repository);
            var request = CreateValidRequest() with { CategoryId = 0 };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(1,request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenUnitPriceIsNegative()
        {
            var repository = new FakeProductCommandRepository(UpdateProductResult.Updated);
            var handler = new UpdateProductHandler(repository);
            var request = CreateValidRequest() with { UnitPrice = -10m };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(1, request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenMinimumStockIsNegative()
        {
            var repository = new FakeProductCommandRepository(UpdateProductResult.Updated);
            var handler = new UpdateProductHandler(repository);
            var request = CreateValidRequest() with { MinimumStock = -1 };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(1, request, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        [InlineData(9)]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenRowVersionLengthIsInvalid(
            int length)
        {
            var repository = new FakeProductCommandRepository(UpdateProductResult.Updated);
            var handler = new UpdateProductHandler(repository);
            var request = CreateValidRequest() with
            {
                RowVersion = new byte[length]
            };

            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.HandleAsync(1, request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowArgumentException_WhenRowVersionIsNull()
        {
            var repository = new FakeProductCommandRepository(UpdateProductResult.Updated);
            var handler = new UpdateProductHandler(repository);
            var request = CreateValidRequest() with { RowVersion = null! };

            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.HandleAsync(
                    1,
                    request,
                    CancellationToken.None));
        }

        private static UpdateProductRequest CreateValidRequest()
        {
            return new UpdateProductRequest(
                CategoryId: 1,
                Sku: "ELEC-0001",
                Name: "Wireless Mouse",
                Description: "Ergonomic wireless mouse",
                UnitPrice: 349.90m,
                MinimumStock: 5,
                RowVersion: new byte[8]);
        }

        private sealed class FakeProductCommandRepository : IProductCommandRepository
        {
            private readonly UpdateProductResult _result;

            public int? ReceivedProductId { get; private set; }
            public UpdateProductRequest? ReceivedUpdateRequest { get; private set; }

            public FakeProductCommandRepository(UpdateProductResult result) => _result = result;
            public Task<int?> CreateAsync(
                CreateProductRequest request,
                CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task<UpdateProductResult> UpdateAsync(
                int productId,
                UpdateProductRequest request,
                CancellationToken cancellationToken)
            {
                ReceivedProductId = productId;
                ReceivedUpdateRequest = request;

                return Task.FromResult(_result);
            }

            public Task<bool> DeactivateAsync(
                int productId,
                CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}