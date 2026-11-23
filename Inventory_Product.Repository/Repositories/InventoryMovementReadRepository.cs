using Dapper;
using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.InventoryMovements.DTOs;
namespace Inventory_Product.Repository.Repositories
{
    public class InventoryMovementReadRepository : IInventoryMovementReadRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public InventoryMovementReadRepository(ISqlConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;
        public async Task<IReadOnlyList<InventoryMovementResponse>> GetByProductIdAsync(
            int productId,
            CancellationToken cancellationToken)
        {
            const string sql = """
            SELECT
                movement.MovementId,
                movement.ProductId,
                product.Name AS ProductName,
                movement.MovementTypeId,
                movementType.Name AS MovementType,
                movement.Quantity,
                movement.Reason,
                movement.CreatedBy,
                movement.CreatedAt
            FROM dbo.InventoryMovements AS movement
            INNER JOIN dbo.Products AS product
                ON product.ProductId = movement.ProductId
            INNER JOIN dbo.MovementTypes AS movementType
                ON movementType.MovementTypeId = movement.MovementTypeId
            WHERE movement.ProductId = @ProductId
            ORDER BY movement.CreatedAt DESC, movement.MovementId DESC;
            """;
            await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
            var movements = await connection.QueryAsync<InventoryMovementResponse>(
                new CommandDefinition(
                    sql,
                    new { ProductId = productId },
                    cancellationToken: cancellationToken));
            return movements.AsList();
        }
    }
}