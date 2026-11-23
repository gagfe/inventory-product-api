namespace Inventory_Product.Test.Integration
{
    public class SqlServerConnectionTests
    {
        [Fact]
        public async Task CreateConnectionAsync_ShouldConnectToTestDatabase()
        {
            var factory = new SqlServerTestConnectionFactory();
            await using var connection = await factory.CreateConnectionAsync(CancellationToken.None);
            Assert.Equal("InventoryDB_Test", connection.Database);
            Assert.Equal(System.Data.ConnectionState.Open,connection.State);
        }
    }
}