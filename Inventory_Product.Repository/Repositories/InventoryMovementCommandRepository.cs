using Dapper;
using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.InventoryMovements.DTOs;
namespace Inventory_Product.Repository.Repositories
{
    public class InventoryMovementCommandRepository : IInventoryMovementCommandRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public InventoryMovementCommandRepository(ISqlConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;
        public async Task<RegisterInventoryMovementResult> RegisterAsync(
            RegisterInventoryMovementRequest request,
            string createdBy,
            CancellationToken cancellationToken)
        {
            await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
            try
            {
                var stockSign = await GetStockSignAsync(
                    connection, transaction, request.MovementTypeId, cancellationToken);
                if (stockSign is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return RegisterInventoryMovementResult.MovementTypeNotFound;
                }
                var productExists = await ProductExistsAsync(
                    connection, transaction, request.ProductId, cancellationToken);
                if (!productExists)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return RegisterInventoryMovementResult.ProductNotFound;
                }
                var stockUpdated = await UpdateStockAsync(
                    connection, transaction, request, stockSign.Value, cancellationToken);
                if (!stockUpdated)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return RegisterInventoryMovementResult.InsufficientStock;
                }
                await InsertMovementAsync(connection, transaction, request, createdBy ,cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return RegisterInventoryMovementResult.Success;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        private static Task<short?> GetStockSignAsync(
            System.Data.Common.DbConnection connection,
            System.Data.Common.DbTransaction transaction,
            byte movementTypeId,
            CancellationToken cancellationToken)
        {
            const string sql = """
            SELECT StockSign
            FROM dbo.MovementTypes
            WHERE MovementTypeId = @MovementTypeId;
            """;
            return connection.QuerySingleOrDefaultAsync<short?>(
                new CommandDefinition(
                    sql,
                    new { MovementTypeId = movementTypeId },
                    transaction,
                    cancellationToken: cancellationToken));
        }
        private static Task<bool> ProductExistsAsync(
            System.Data.Common.DbConnection connection,
            System.Data.Common.DbTransaction transaction,
            int productId,
            CancellationToken cancellationToken)
        {
            const string sql = """
            SELECT CAST(CASE WHEN EXISTS (
                SELECT 1
                FROM dbo.Products WITH (UPDLOCK, HOLDLOCK)
                WHERE ProductId = @ProductId AND IsActive = 1
            ) THEN 1 ELSE 0 END AS BIT);
            """;
            return connection.QuerySingleAsync<bool>(
                new CommandDefinition(
                    sql,
                    new { ProductId = productId },
                    transaction,
                    cancellationToken: cancellationToken));
        }
        private static async Task<bool> UpdateStockAsync(
            System.Data.Common.DbConnection connection,
            System.Data.Common.DbTransaction transaction,
            RegisterInventoryMovementRequest request,
            short stockSign,
            CancellationToken cancellationToken)
        {
            const string sql = """
            UPDATE dbo.Products
            SET CurrentStock = CurrentStock + (@Quantity * @StockSign),
                UpdatedAt = SYSUTCDATETIME()
            WHERE ProductId = @ProductId
                AND IsActive = 1
                AND (@StockSign = 1 OR CurrentStock >= @Quantity);
            """;
            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new
                    {
                        request.ProductId,
                        request.Quantity,
                        StockSign = stockSign
                    },
                    transaction,
                    cancellationToken: cancellationToken));
            return affectedRows == 1;
        }
        private static Task<int> InsertMovementAsync(
            System.Data.Common.DbConnection connection,
            System.Data.Common.DbTransaction transaction,
            RegisterInventoryMovementRequest request,
            string createdBy,
            CancellationToken cancellationToken)
        {
            const string sql = """
            INSERT INTO dbo.InventoryMovements
                (ProductId, MovementTypeId, Quantity, Reason, CreatedBy)
            VALUES
                (@ProductId, @MovementTypeId, @Quantity, @Reason, @CreatedBy);
            """;
            return connection.ExecuteAsync(new CommandDefinition(sql,
            new
            {
                request.ProductId,
                request.MovementTypeId,
                request.Quantity,
                request.Reason,
                CreatedBy = createdBy
            },
            transaction,
            cancellationToken: cancellationToken));
        }
    }
}