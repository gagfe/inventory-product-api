using Dapper;
using Microsoft.Data.SqlClient;
using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Exceptions;
using Inventory_Product.Application.Products.Commands.UpdateProduct;
using Inventory_Product.Application.Products.DTOs;

namespace Inventory_Product.Repository.Repositories
{
    public class ProductCommandRepository : IProductCommandRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ProductCommandRepository(ISqlConnectionFactory connectionFactory)
            => _connectionFactory = connectionFactory;

        public async Task<int?> CreateAsync(
            CreateProductRequest request,
            CancellationToken cancellationToken)
        {
            const string sql = """
            INSERT INTO dbo.Products
                (CategoryId, Sku, Name, Description, UnitPrice, CurrentStock, MinimumStock, IsActive)
            OUTPUT INSERTED.ProductId
            SELECT
                @CategoryId, @Sku, @Name, @Description, @UnitPrice, 0, @MinimumStock, 1
            FROM dbo.Categories
            WHERE CategoryId = @CategoryId AND IsActive = 1;
            """;

            await using var connection =
                await _connectionFactory.CreateConnectionAsync(cancellationToken);

            try
            {
                return await connection.QuerySingleOrDefaultAsync<int?>(
                    new CommandDefinition(
                        sql,
                        request,
                        cancellationToken: cancellationToken));
            }
            catch (SqlException ex) when (IsDuplicateKeyException(ex))
            {
                throw new DuplicateResourceException(
                    "A product with the same SKU already exists.");
            }
        }


        public async Task<UpdateProductResult> UpdateAsync(
            int productId,
            UpdateProductRequest request,
            CancellationToken cancellationToken)
        {
            const string sql = """
                SET NOCOUNT ON;

                DECLARE @Result INT;

                BEGIN TRANSACTION;

                BEGIN TRY
                    IF NOT EXISTS (
                        SELECT 1
                        FROM dbo.Products WITH (UPDLOCK, HOLDLOCK)
                        WHERE ProductId = @ProductId
                            AND IsActive = 1
                    )
                    BEGIN
                        SET @Result = 1;
                    END
                    ELSE IF NOT EXISTS (
                        SELECT 1
                        FROM dbo.Categories
                        WHERE CategoryId = @CategoryId
                            AND IsActive = 1
                    )
                    BEGIN
                        SET @Result = 2;
                    END
                    ELSE
                    BEGIN
                        UPDATE dbo.Products
                        SET
                            CategoryId = @CategoryId,
                            Sku = @Sku,
                            Name = @Name,
                            Description = @Description,
                            UnitPrice = @UnitPrice,
                            MinimumStock = @MinimumStock,
                            UpdatedAt = SYSUTCDATETIME()
                        WHERE ProductId = @ProductId
                            AND IsActive = 1
                            AND RowVersion = @RowVersion;

                        IF @@ROWCOUNT = 0
                            SET @Result = 3;
                        ELSE
                            SET @Result = 0;
                    END

                    COMMIT TRANSACTION;

                    SELECT @Result;
                END TRY
                BEGIN CATCH
                    IF @@TRANCOUNT > 0
                        ROLLBACK TRANSACTION;

                    THROW;
                END CATCH;
                """;

            await using var connection =
                await _connectionFactory.CreateConnectionAsync(cancellationToken);

            try
            {
                var result = await connection.QuerySingleAsync<int>(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            ProductId = productId,
                            request.CategoryId,
                            request.Sku,
                            request.Name,
                            request.Description,
                            request.UnitPrice,
                            request.MinimumStock,
                            request.RowVersion
                        },
                        cancellationToken: cancellationToken));

                return result switch
                {
                    0 => UpdateProductResult.Updated,
                    1 => UpdateProductResult.NotFound,
                    2 => UpdateProductResult.InvalidCategory,
                    3 => UpdateProductResult.ConcurrencyConflict,
                    _ => throw new InvalidOperationException("An unexpected product update result was returned.")
                };
            }
            catch (SqlException ex) when (IsDuplicateKeyException(ex))
            {
                throw new DuplicateResourceException("A product with the same SKU already exists.");
            }
        }

        public async Task<bool> DeactivateAsync(
            int productId,
            CancellationToken cancellationToken)
        {
            const string sql = """
            UPDATE dbo.Products
            SET IsActive = 0, UpdatedAt = SYSUTCDATETIME()
            WHERE ProductId = @ProductId AND IsActive = 1;
            """;

            await using var connection =
                await _connectionFactory.CreateConnectionAsync(cancellationToken);

            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new { ProductId = productId },
                    cancellationToken: cancellationToken));

            return affectedRows > 0;
        }

        private static bool IsDuplicateKeyException(SqlException exception) => exception.Number is 2601 or 2627;
    }
}