using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
namespace Inventory_Product.Domain.Entities
{
    public class InventoryMovement
    {
        public long MovementId { get; private set; }
        public int ProductId { get; private set; }
        public byte MovementTypeId { get; private set; }
        public int Quantity { get; private set; }
        public string? Reason { get; private set; }
        public string CreatedBy { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        private InventoryMovement() { }
        public InventoryMovement(int productId, byte movementTypeId, int quantity, string? reason, string createdBy)
        {
            ValidateProductId(productId);
            ValidateMovementTypeId(movementTypeId);
            ValidateQuantity(quantity);
            ValidateReason(reason);
            ValidateCreatedBy(createdBy);
            ProductId = productId;
            MovementTypeId = movementTypeId;
            Quantity = quantity;
            Reason = reason?.Trim();
            CreatedBy = createdBy.Trim();
            CreatedAt = DateTime.UtcNow;
        }
        private static void ValidateProductId(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than zero.", nameof(productId));
        }
        private static void ValidateMovementTypeId(byte movementTypeId)
        {
            if (movementTypeId == 0)
                throw new ArgumentException("Movement type ID must be greater than zero.", nameof(movementTypeId));
        }
        private static void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }
        private static void ValidateReason(string? reason)
        {
            if (reason?.Length > 250)
                throw new ArgumentException("Reason cannot exceed 250 characters.", nameof(reason));
        }
        private static void ValidateCreatedBy(string createdBy)
        {
            if (string.IsNullOrWhiteSpace(createdBy))
                throw new ArgumentException("Created by is required.", nameof(createdBy));
            if (createdBy.Trim().Length > 100)
                throw new ArgumentException("Created by cannot exceed 100 characters.", nameof(createdBy));
        }
    }
}