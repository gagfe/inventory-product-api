using Inventory_Product.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory_Product.Test.Entities
{
    public class MovementTypeTests
    {
        [Fact]
        public void Constructor_WithValidEntry_ShouldCreateMovementType()
        {
            var movementType = new MovementType(1, "Entry", 1);
            Assert.Equal(1, movementType.MovementTypeId);
            Assert.Equal("Entry", movementType.Name);
            Assert.Equal(1, movementType.StockSign);
        }
        [Fact]
        public void Constructor_WithValidExit_ShouldCreateMovementType()
        {
            var movementType = new MovementType(2, "Exit", -1);
            Assert.Equal(2, movementType.MovementTypeId);
            Assert.Equal("Exit", movementType.Name);
            Assert.Equal(-1, movementType.StockSign);
        }
        [Fact]
        public void Constructor_WithZeroId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MovementType(0, "Entry", 1));
        }
        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MovementType(1, "", 1));
        }
        [Fact]
        public void Constructor_WithNameExceeding30Characters_ShouldThrowArgumentException()
        {
            var name = new string('A', 31);
            Assert.Throws<ArgumentException>(() => new MovementType(1, name, 1));
        }
        [Fact]
        public void Constructor_WithZeroStockSign_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MovementType(1, "Entry", 0));
        }
        [Fact]
        public void Constructor_WithInvalidStockSign_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MovementType(1, "Entry", 2));
        }
    }
}
