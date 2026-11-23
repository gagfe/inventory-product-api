using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Inventory_Product.Domain.Entities
{
    public class MovementType
    {
        public byte MovementTypeId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public short StockSign { get; private set; }
        private MovementType() { }
        public MovementType(byte movementTypeId, string name, short stockSign)
        {
            ValidateMovementTypeId(movementTypeId);
            ValidateName(name);
            ValidateStockSign(stockSign);
            MovementTypeId = movementTypeId;
            Name = name.Trim();
            StockSign = stockSign;
        }
        private static void ValidateMovementTypeId(byte movementTypeId)
        {
            if (movementTypeId == 0)
                throw new ArgumentException("Movement type ID must be greater than zero.", nameof(movementTypeId));
        }
        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Movement type name is required.", nameof(name));
            if (name.Trim().Length > 30)
                throw new ArgumentException("Movement type name cannot exceed 30 characters.", nameof(name));
        }
        private static void ValidateStockSign(short stockSign)
        {
            if (stockSign != -1 && stockSign != 1)
                throw new ArgumentException("Stock sign must be either -1 or 1.", nameof(stockSign));
        }
    }
}