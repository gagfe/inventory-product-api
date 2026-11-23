using System.Data.Common;
using Inventory_Product.Application.Abstraction.Persistence;
using Microsoft.Data.SqlClient;

namespace Inventory_Product.Test.Integration
{
    public sealed class SqlServerTestConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;
        public SqlServerTestConnectionFactory()
        {
            var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("The integration test connection string is not configured.");

            var builder = new SqlConnectionStringBuilder(connectionString);

            if (!string.Equals(builder.InitialCatalog, "InventoryDB_Test", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Integration tests must use InventoryDB_Test.");

            _connectionString = builder.ConnectionString;
        }

        public async Task<DbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            if (!string.Equals(connection.Database, "InventoryDB_Test", StringComparison.OrdinalIgnoreCase))
            {
                await connection.DisposeAsync();
                throw new InvalidOperationException("The connection is not using InventoryDB_Test.");
            }
            return connection;
        }
    }
}