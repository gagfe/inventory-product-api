using Inventory_Product.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory_Product.Test.Entities
{
    public class InventoryMovementTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateMovement()
        {
            var movement = new InventoryMovement(1, 1, 10, "Initial stock", "admin");
            Assert.Equal(1, movement.ProductId);
            Assert.Equal(1, movement.MovementTypeId);
            Assert.Equal(10, movement.Quantity);
            Assert.Equal("Initial stock", movement.Reason);
            Assert.Equal("admin", movement.CreatedBy);
            Assert.NotEqual(default, movement.CreatedAt);
        }
        [Fact]
        public void Constructor_WithInvalidProductId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new InventoryMovement(0, 1, 10, null, "admin"));
        }
        [Fact]
        public void Constructor_WithZeroMovementTypeId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new InventoryMovement(1, 0, 10, null, "admin"));
        }
        [Fact]
        public void Constructor_WithZeroQuantity_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new InventoryMovement(1, 1, 0, null, "admin"));
        }
        [Fact]
        public void Constructor_WithNegativeQuantity_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new InventoryMovement(1, 1, -5, null, "admin"));
        }
        [Fact]
        public void Constructor_WithReasonExceeding250Characters_ShouldThrowArgumentException()
        {
            var reason = new string('A', 251);
            Assert.Throws<ArgumentException>(() => new InventoryMovement(1, 1, 10, reason, "admin"));
        }
        [Fact]
        public void Constructor_WithEmptyCreatedBy_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new InventoryMovement(1, 1, 10, null, ""));
        }
        [Fact]
        public void Constructor_WithCreatedByExceeding100Characters_ShouldThrowArgumentException()
        {
            var createdBy = new string('A', 101);
            Assert.Throws<ArgumentException>(() => new InventoryMovement(1, 1, 10, null, createdBy));
        }
        [Fact]
        public void Constructor_WithNullReason_ShouldCreateMovement()
        {
            var movement = new InventoryMovement(1, 1, 10, null, "admin");
            Assert.Null(movement.Reason);
        }
    }
}
