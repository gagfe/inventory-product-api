using Inventory_Product.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory_Product.Test.Entities
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateProduct()
        {
            var product = new Product(1, "ELEC-0001", "Wireless Mouse", "Wireless device", 349.90m, 5);
            Assert.Equal(1, product.CategoryId);
            Assert.Equal("ELEC-0001", product.Sku);
            Assert.Equal("Wireless Mouse", product.Name);
            Assert.Equal(349.90m, product.UnitPrice);
            Assert.Equal(0, product.CurrentStock);
            Assert.Equal(5, product.MinimumStock);
            Assert.True(product.IsActive);
            Assert.Null(product.UpdatedAt);
        }
        [Fact]
        public void Constructor_WithInvalidCategoryId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Product(0, "ELEC-0001", "Mouse", null, 100m, 5));
        }
        [Fact]
        public void Constructor_WithEmptySku_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Product(1, "", "Mouse", null, 100m, 5));
        }
        [Fact]
        public void Constructor_WithSkuExceeding50Characters_ShouldThrowArgumentException()
        {
            var sku = new string('A', 51);
            Assert.Throws<ArgumentException>(() => new Product(1, sku, "Mouse", null, 100m, 5));
        }
        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Product(1, "ELEC-0001", " ", null, 100m, 5));
        }
        [Fact]
        public void Constructor_WithDescriptionExceeding1000Characters_ShouldThrowArgumentException()
        {
            var description = new string('A', 1001);
            Assert.Throws<ArgumentException>(() => new Product(1, "ELEC-0001", "Mouse", description, 100m, 5));
        }
        [Fact]
        public void Constructor_WithNegativeUnitPrice_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Product(1, "ELEC-0001", "Mouse", null, -1m, 5));
        }
        [Fact]
        public void Constructor_WithNegativeMinimumStock_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Product(1, "ELEC-0001", "Mouse", null, 100m, -1));
        }
        [Fact]
        public void AddStock_WithValidQuantity_ShouldIncreaseCurrentStock()
        {
            var product = new Product(1, "ELEC-0001", "Mouse", null, 100m, 5);
            product.AddStock(10);
            Assert.Equal(10, product.CurrentStock);
        }
        [Fact]
        public void AddStock_WithZeroQuantity_ShouldThrowArgumentException()
        {
            var product = new Product(1, "ELEC-0001", "Mouse", null, 100m, 5);
            Assert.Throws<ArgumentException>(() => product.AddStock(0));
        }
        [Fact]
        public void AddStock_WithOverflow_ShouldThrowOverflowException()
        {
            var product = new Product(1, "ELEC-0001", "Mouse", null, 100m, 5);
            product.AddStock(int.MaxValue);
            Assert.Throws<OverflowException>(() => product.AddStock(1));
        }
        [Fact]
        public void RemoveStock_WithValidQuantity_ShouldDecreaseCurrentStock()
        {
            var product = new Product(1, "ELEC-0001", "Mouse", null, 100m, 5);
            product.AddStock(10);
            product.RemoveStock(4);
            Assert.Equal(6, product.CurrentStock);
        }
        [Fact]
        public void RemoveStock_WithInsufficientStock_ShouldThrowInvalidOperationException()
        {
            var product = new Product(1, "ELEC-0001", "Mouse", null, 100m, 5);
            Assert.Throws<InvalidOperationException>(() => product.RemoveStock(1));
        }
        [Fact]
        public void UpdateUnitPrice_WithNegativePrice_ShouldThrowArgumentException()
        {
            var product = new Product(1, "ELEC-0001", "Mouse", null, 100m, 5);
            Assert.Throws<ArgumentException>(() => product.UpdateUnitPrice(-1m));
        }
        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            var product = new Product(1, "ELEC-0001", "Mouse", null, 100m, 5);
            product.Deactivate();
            Assert.False(product.IsActive);
            Assert.NotNull(product.UpdatedAt);
        }
        [Fact]
        public void Activate_ShouldSetIsActiveToTrue()
        {
            var product = new Product(1, "ELEC-0001", "Mouse", null, 100m, 5);
            product.Deactivate();
            product.Activate();
            Assert.True(product.IsActive);
        }
    }
}
