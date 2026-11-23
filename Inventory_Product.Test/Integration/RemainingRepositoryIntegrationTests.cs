using Dapper;
using Inventory_Product.Application.Categories.DTOs;
using Inventory_Product.Application.Exceptions;
using Inventory_Product.Application.Products.Commands.UpdateProduct;
using Inventory_Product.Application.Products.DTOs;
using Inventory_Product.Repository.Persistence;
using Inventory_Product.Repository.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Product.Test.Integration
{
    public class RemainingRepositoryIntegrationTests
    {
        private readonly SqlServerTestConnectionFactory _factory = new();

        [Fact]
        public async Task UpdateProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var categoryId = await CreateCategoryAsync();

            try
            {
                var repository = new ProductCommandRepository(_factory);
                var request = CreateUpdateRequest(
                    categoryId,
                    UniqueSku(),
                    new byte[8]);

                var result = await repository.UpdateAsync(
                    -1,
                    request,
                    CancellationToken.None);

                Assert.Equal(UpdateProductResult.NotFound, result);
            }
            finally
            {
                await CleanupAsync(categoryId);
            }
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnInvalidCategory_WhenCategoryDoesNotExist()
        {
            var categoryId = await CreateCategoryAsync();

            try
            {
                var repository = new ProductCommandRepository(_factory);
                var sku = UniqueSku();
                var productId = await CreateProductAsync(categoryId, sku);
                var rowVersion = await GetRowVersionAsync(productId);

                var request = CreateUpdateRequest(
                    -1,
                    sku,
                    rowVersion);

                var result = await repository.UpdateAsync(
                    productId,
                    request,
                    CancellationToken.None);

                Assert.Equal(UpdateProductResult.InvalidCategory, result);
            }
            finally
            {
                await CleanupAsync(categoryId);
            }
        }

        [Fact]
        public async Task CreateProduct_ShouldThrowDuplicateResourceException_WhenSkuExists()
        {
            var categoryId = await CreateCategoryAsync();

            try
            {
                var repository = new ProductCommandRepository(_factory);
                var sku = UniqueSku();

                await CreateProductAsync(categoryId, sku);

                var duplicateRequest = CreateProductRequest(categoryId, sku);

                await Assert.ThrowsAsync<DuplicateResourceException>(
                    () => repository.CreateAsync(
                        duplicateRequest,
                        CancellationToken.None));
            }
            finally
            {
                await CleanupAsync(categoryId);
            }
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowDuplicateResourceException_WhenSkuExists()
        {
            var categoryId = await CreateCategoryAsync();

            try
            {
                var repository = new ProductCommandRepository(_factory);
                var existingSku = UniqueSku();
                var secondSku = UniqueSku();

                await CreateProductAsync(categoryId, existingSku);
                var secondProductId = await CreateProductAsync(
                    categoryId,
                    secondSku);

                var rowVersion = await GetRowVersionAsync(secondProductId);

                var request = CreateUpdateRequest(
                    categoryId,
                    existingSku,
                    rowVersion);

                await Assert.ThrowsAsync<DuplicateResourceException>(
                    () => repository.UpdateAsync(
                        secondProductId,
                        request,
                        CancellationToken.None));
            }
            finally
            {
                await CleanupAsync(categoryId);
            }
        }

        [Fact]
        public async Task CreateCategory_ShouldThrowDuplicateResourceException_WhenNameExists()
        {
            var categoryName = UniqueCategoryName();
            var categoryId = await CreateCategoryAsync(categoryName);

            try
            {
                var repository = new CategoryCommandRepository(_factory);

                var request = new CreateCategoryRequest(
                    categoryName,
                    "Duplicate category test");

                await Assert.ThrowsAsync<DuplicateResourceException>(
                    () => repository.CreateAsync(
                        request,
                        CancellationToken.None));
            }
            finally
            {
                await CleanupAsync(categoryId);
            }
        }

        [Fact]
        public async Task UpdateCategory_ShouldThrowDuplicateResourceException_WhenNameExists()
        {
            var firstName = UniqueCategoryName();
            var firstId = await CreateCategoryAsync(firstName);
            int? secondId = null;

            try
            {
                secondId = await CreateCategoryAsync();

                var repository = new CategoryCommandRepository(_factory);

                var request = new UpdateCategoryRequest(
                    firstName,
                    "Duplicate category update test");

                await Assert.ThrowsAsync<DuplicateResourceException>(
                    () => repository.UpdateAsync(
                        secondId.Value,
                        request,
                        CancellationToken.None));
            }
            finally
            {
                if (secondId.HasValue)
                    await CleanupAsync(secondId.Value);

                await CleanupAsync(firstId);
            }
        }

        [Fact]
        public async Task GetProducts_ShouldExcludeInactiveProducts()
        {
            var categoryId = await CreateCategoryAsync();

            try
            {
                var activeSku = UniqueSku();
                var inactiveSku = UniqueSku();

                await CreateProductAsync(categoryId, activeSku);
                var inactiveId = await CreateProductAsync(
                    categoryId,
                    inactiveSku);

                var commandRepository = new ProductCommandRepository(_factory);

                var deactivated = await commandRepository.DeactivateAsync(
                    inactiveId,
                    CancellationToken.None);

                Assert.True(deactivated);

                await using var connection =
                    await _factory.CreateConnectionAsync(CancellationToken.None);

                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(connection)
                    .Options;

                await using var context = new AppDbContext(options);

                var readRepository = new ProductReadRepository(context);
                var products = await readRepository.GetAllAsync(
                    CancellationToken.None);

                Assert.Contains(products, product => product.Sku == activeSku);
                Assert.DoesNotContain(products, product => product.Sku == inactiveSku);
            }
            finally
            {
                await CleanupAsync(categoryId);
            }
        }

        [Fact]
        public async Task GetCategories_ShouldExcludeInactiveCategories()
        {
            var activeName = UniqueCategoryName();
            var inactiveName = UniqueCategoryName();

            var activeId = await CreateCategoryAsync(activeName);
            int? inactiveId = null;

            try
            {
                inactiveId = await CreateCategoryAsync(inactiveName);

                var commandRepository = new CategoryCommandRepository(_factory);

                var deactivated = await commandRepository.DeactivateAsync(
                    inactiveId.Value,
                    CancellationToken.None);

                Assert.True(deactivated);

                await using var connection =
                    await _factory.CreateConnectionAsync(CancellationToken.None);

                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(connection)
                    .Options;

                await using var context = new AppDbContext(options);

                var readRepository = new CategoryReadRepository(context);
                var categories = await readRepository.GetAllAsync(
                    CancellationToken.None);

                var inactiveCategory = await readRepository.GetByIdAsync(
                    inactiveId.Value,
                    CancellationToken.None);

                Assert.Contains(
                    categories,
                    category => category.CategoryId == activeId);

                Assert.DoesNotContain(
                    categories,
                    category => category.CategoryId == inactiveId.Value);

                Assert.Null(inactiveCategory);
            }
            finally
            {
                if (inactiveId.HasValue)
                    await CleanupAsync(inactiveId.Value);

                await CleanupAsync(activeId);
            }
        }

        [Fact]
        public async Task GetMovements_ShouldPreserveHistory_WhenProductIsInactive()
        {
            var categoryId = await CreateCategoryAsync();

            try
            {
                var productId = await CreateProductAsync(
                    categoryId,
                    UniqueSku());

                await SeedMovementTypeAsync();
                await CreateMovementAsync(productId);

                var commandRepository = new ProductCommandRepository(_factory);

                var deactivated = await commandRepository.DeactivateAsync(
                    productId,
                    CancellationToken.None);

                Assert.True(deactivated);

                var readRepository = new InventoryMovementReadRepository(_factory);

                var movements = await readRepository.GetByProductIdAsync(
                    productId,
                    CancellationToken.None);

                Assert.Contains(
                    movements,
                    movement =>
                        movement.ProductId == productId &&
                        movement.Quantity == 10);
            }
            finally
            {
                await CleanupAsync(categoryId);
            }
        }

        private async Task<int> CreateCategoryAsync(string? name = null)
        {
            const string sql = """
            INSERT INTO dbo.Categories (Name, Description, IsActive)
            OUTPUT INSERTED.CategoryId
            VALUES (@Name, @Description, 1);
            """;

            await using var connection =
                await _factory.CreateConnectionAsync(CancellationToken.None);

            return await connection.QuerySingleAsync<int>(
                sql,
                new
                {
                    Name = name ?? UniqueCategoryName(),
                    Description = "Integration test category"
                });
        }

        private async Task<int> CreateProductAsync(int categoryId, string sku)
        {
            var repository = new ProductCommandRepository(_factory);

            var result = await repository.CreateAsync(
                CreateProductRequest(categoryId, sku),
                CancellationToken.None);

            Assert.NotNull(result);

            return result.Value;
        }

        private static CreateProductRequest CreateProductRequest(
            int categoryId,
            string sku)
        {
            return new CreateProductRequest(
                CategoryId: categoryId,
                Sku: sku,
                Name: "Integration Test Product",
                Description: "Created by integration test",
                UnitPrice: 100m,
                MinimumStock: 5);
        }

        private static UpdateProductRequest CreateUpdateRequest(
            int categoryId,
            string sku,
            byte[] rowVersion)
        {
            return new UpdateProductRequest(
                CategoryId: categoryId,
                Sku: sku,
                Name: "Updated Integration Product",
                Description: "Updated by integration test",
                UnitPrice: 150m,
                MinimumStock: 5,
                RowVersion: rowVersion);
        }

        private async Task<byte[]> GetRowVersionAsync(int productId)
        {
            const string sql = """
            SELECT RowVersion
            FROM dbo.Products
            WHERE ProductId = @ProductId;
            """;

            await using var connection =
                await _factory.CreateConnectionAsync(CancellationToken.None);

            return await connection.QuerySingleAsync<byte[]>(
                sql,
                new { ProductId = productId });
        }

        private async Task SeedMovementTypeAsync()
        {
            const string sql = """
            IF NOT EXISTS (
                SELECT 1
                FROM dbo.MovementTypes
                WHERE MovementTypeId = 1
            )
            BEGIN
                INSERT INTO dbo.MovementTypes
                    (MovementTypeId, Name, StockSign)
                VALUES (1, 'Entry', 1);
            END;
            """;

            await using var connection =
                await _factory.CreateConnectionAsync(CancellationToken.None);

            await connection.ExecuteAsync(sql);
        }

        private async Task CreateMovementAsync(int productId)
        {
            const string sql = """
            INSERT INTO dbo.InventoryMovements
                (ProductId, MovementTypeId, Quantity, Reason, CreatedBy)
            VALUES
                (@ProductId, 1, 10, @Reason, @CreatedBy);
            """;

            await using var connection =
                await _factory.CreateConnectionAsync(CancellationToken.None);

            await connection.ExecuteAsync(
                sql,
                new
                {
                    ProductId = productId,
                    Reason = "Inactive product history test",
                    CreatedBy = "integration.test"
                });
        }

        private async Task CleanupAsync(int categoryId)
        {
            const string sql = """
            DELETE movement
            FROM dbo.InventoryMovements AS movement
            INNER JOIN dbo.Products AS product
                ON product.ProductId = movement.ProductId
            WHERE product.CategoryId = @CategoryId;

            DELETE FROM dbo.Products
            WHERE CategoryId = @CategoryId;

            DELETE FROM dbo.Categories
            WHERE CategoryId = @CategoryId;
            """;

            await using var connection =
                await _factory.CreateConnectionAsync(CancellationToken.None);

            await connection.ExecuteAsync(
                sql,
                new { CategoryId = categoryId });
        }

        private static string UniqueSku()
        {
            return $"TEST-{Guid.NewGuid():N}";
        }

        private static string UniqueCategoryName()
        {
            return $"Test-{Guid.NewGuid():N}";
        }
    }
}
