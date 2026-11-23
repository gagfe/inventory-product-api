using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
namespace Inventory_Product.Application.Abstraction.Persistence
{
    public interface ISqlConnectionFactory
    {
        Task<DbConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
}