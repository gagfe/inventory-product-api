using Inventory_Product.Application.Abstraction.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
namespace Inventory_Product.Repository.Connections
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;
        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("The database connection string is not configured.");
        }
        public async Task<DbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }
}