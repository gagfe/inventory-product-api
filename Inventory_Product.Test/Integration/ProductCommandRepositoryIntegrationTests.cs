using Dapper;
using Inventory_Product.Application.Products.Commands.UpdateProduct;
using Inventory_Product.Application.Products.DTOs;
using Inventory_Product.Repository.Repositories;

namespace Inventory_Product.Test.Integration
{
    public class ProductCommandRepositoryIntegrationTests
    {
        private readonly SqlServerTestConnectionFactory _connectionFactory = new();

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct_WhenRowVersionMatches()
        {
            var categoryId = await CreateTestCategoryAsync();
            try
            {
                var repository = new ProductCommandRepository(_connectionFactory);
                var sku = CreateUniqueSku();
                var productId = await CreateTestProductAsync(repository, categoryId, sku);
                var rowVersion = await GetRowVersionAsync(productId);
                var request = CreateUpdateRequest(categoryId, sku, rowVersion, "Updated Product");

                var result = await repository.UpdateAsync(
                    productId,
                    request,
                    CancellationToken.None);

                var updatedRowVersion = await GetRowVersionAsync(productId);
                var productName = await GetProductNameAsync(productId);

                Assert.Equal(UpdateProductResult.Updated, result);
                Assert.Equal("Updated Product", productName);
                Assert.False(rowVersion.SequenceEqual(updatedRowVersion));
            }
            finally
            {
                await DeleteTestDataAsync(categoryId);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnConcurrencyConflict_WhenRowVersionIsOutdated()
        {
            var categoryId = await CreateTestCategoryAsync();
            try
            {
                var repository = new ProductCommandRepository(_connectionFactory);
                var sku = CreateUniqueSku();
                var productId = await CreateTestProductAsync(repository, categoryId, sku);
                var originalRowVersion = await GetRowVersionAsync(productId);

                var firstRequest = CreateUpdateRequest(
                    categoryId,
                    sku,
                    originalRowVersion,
                    "First Update");

                var firstResult = await repository.UpdateAsync(
                    productId,
                    firstRequest,
                    CancellationToken.None);

                var outdatedRequest = CreateUpdateRequest(
                    categoryId,
                    sku,
                    originalRowVersion,
                    "Second Update");

                var secondResult = await repository.UpdateAsync(
                    productId,
                    outdatedRequest,
                    CancellationToken.None);

                var productName = await GetProductNameAsync(productId);

                Assert.Equal(UpdateProductResult.Updated, firstResult);
                Assert.Equal(UpdateProductResult.ConcurrencyConflict, secondResult);
                Assert.Equal("First Update", productName);
            }
            finally
            {
                await DeleteTestDataAsync(categoryId);
            }
        }

        private async Task<int> CreateTestCategoryAsync()
        {
            const string sql = """
            INSERT INTO dbo.Categories (Name, Description, IsActive)
            OUTPUT INSERTED.CategoryId
            VALUES (@Name, @Description, 1);
            """;

            await using var connection =
                await _connectionFactory.CreateConnectionAsync(CancellationToken.None);

            return await connection.QuerySingleAsync<int>(
                sql,
                new
                {
                    Name = $"Test-{Guid.NewGuid():N}",
                    Description = "Integration test category"
                });
        }

        private async Task<int> CreateTestProductAsync(
            ProductCommandRepository repository,
            int categoryId,
            string sku)
        {
            var request = new CreateProductRequest(
                CategoryId: categoryId,
                Sku: sku,
                Name: "Original Product",
                Description: "Integration test product",
                UnitPrice: 100m,
                MinimumStock: 5);

            var productId = await repository.CreateAsync(
                request,
                CancellationToken.None);

            Assert.NotNull(productId);

            return productId.Value;
        }

        private async Task<byte[]> GetRowVersionAsync(int productId)
        {
            const string sql = """
            SELECT RowVersion
            FROM dbo.Products
            WHERE ProductId = @ProductId;
            """;

            await using var connection =
                await _connectionFactory.CreateConnectionAsync(CancellationToken.None);

            return await connection.QuerySingleAsync<byte[]>(
                sql,
                new { ProductId = productId });
        }

        private async Task<string> GetProductNameAsync(int productId)
        {
            const string sql = """
            SELECT Name
            FROM dbo.Products
            WHERE ProductId = @ProductId;
            """;

            await using var connection =
                await _connectionFactory.CreateConnectionAsync(CancellationToken.None);

            return await connection.QuerySingleAsync<string>(
                sql,
                new { ProductId = productId });
        }

        private static UpdateProductRequest CreateUpdateRequest(
            int categoryId,
            string sku,
            byte[] rowVersion,
            string name)
        {
            return new UpdateProductRequest(
                CategoryId: categoryId,
                Sku: sku,
                Name: name,
                Description: "Updated through integration test",
                UnitPrice: 150m,
                MinimumStock: 5,
                RowVersion: rowVersion);
        }

        private static string CreateUniqueSku()
        {
            return $"TEST-{Guid.NewGuid():N}";
        }

        private async Task DeleteTestDataAsync(int categoryId)
        {
            const string sql = """
            DELETE FROM dbo.Products
            WHERE CategoryId = @CategoryId;

            DELETE FROM dbo.Categories
            WHERE CategoryId = @CategoryId;
            """;

            await using var connection =
                await _connectionFactory.CreateConnectionAsync(CancellationToken.None);

            await connection.ExecuteAsync(
                sql,
                new { CategoryId = categoryId });
        }
    }
}
