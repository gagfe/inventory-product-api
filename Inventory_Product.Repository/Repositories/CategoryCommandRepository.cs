
using Dapper;
using Microsoft.Data.SqlClient;
using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Categories.DTOs;
using Inventory_Product.Application.Exceptions;

namespace Inventory_Product.Repository.Repositories
{
    public class CategoryCommandRepository : ICategoryCommandRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public CategoryCommandRepository(ISqlConnectionFactory connectionFactory)
            => _connectionFactory = connectionFactory;

        public async Task<int> CreateAsync(
            CreateCategoryRequest request,
            CancellationToken cancellationToken)
        {
            const string sql = """
            INSERT INTO dbo.Categories (Name, Description, IsActive)
            OUTPUT INSERTED.CategoryId
            VALUES (@Name, @Description, 1);
            """;

            await using var connection =
                await _connectionFactory.CreateConnectionAsync(cancellationToken);

            try
            {
                return await connection.QuerySingleAsync<int>(
                    new CommandDefinition(
                        sql,
                        request,
                        cancellationToken: cancellationToken));
            }
            catch (SqlException ex) when (IsDuplicateKeyException(ex))
            {
                throw new DuplicateResourceException(
                    "A category with the same name already exists.");
            }
        }

        public async Task<bool> UpdateAsync(
            int categoryId,
            UpdateCategoryRequest request,
            CancellationToken cancellationToken)
        {
            const string sql = """
            UPDATE dbo.Categories
            SET Name = @Name,
                Description = @Description,
                UpdatedAt = SYSUTCDATETIME()
            WHERE CategoryId = @CategoryId AND IsActive = 1;
            """;

            await using var connection =
                await _connectionFactory.CreateConnectionAsync(cancellationToken);

            try
            {
                var affectedRows = await connection.ExecuteAsync(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            CategoryId = categoryId,
                            request.Name,
                            request.Description
                        },
                        cancellationToken: cancellationToken));

                return affectedRows > 0;
            }
            catch (SqlException ex) when (IsDuplicateKeyException(ex))
            {
                throw new DuplicateResourceException(
                    "A category with the same name already exists.");
            }
        }

        public async Task<bool> DeactivateAsync(
            int categoryId,
            CancellationToken cancellationToken)
        {
            const string sql = """
            UPDATE dbo.Categories
            SET IsActive = 0,
                UpdatedAt = SYSUTCDATETIME()
            WHERE CategoryId = @CategoryId
                AND IsActive = 1
                AND NOT EXISTS (
                    SELECT 1
                    FROM dbo.Products
                    WHERE CategoryId = @CategoryId
                        AND IsActive = 1
                );
            """;
            await using var connection =
                await _connectionFactory.CreateConnectionAsync(cancellationToken);
            var affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new { CategoryId = categoryId },
                    cancellationToken: cancellationToken));

            return affectedRows > 0;
        }

        private static bool IsDuplicateKeyException(SqlException exception) => exception.Number is 2601 or 2627;
    }
}