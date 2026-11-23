using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
namespace Inventory_Product.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; private set; }
        public int CategoryId { get; private set; }
        public string Sku { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int CurrentStock { get; private set; }
        public int MinimumStock { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
        private Product() { }
        public Product(int categoryId, string sku, string name, string? description, decimal unitPrice, int minimumStock)
        {
            ValidateCategoryId(categoryId);
            ValidateSku(sku);
            ValidateName(name);
            ValidateDescription(description);
            ValidateUnitPrice(unitPrice);
            ValidateMinimumStock(minimumStock);
            CategoryId = categoryId;
            Sku = sku;
            Name = name;
            Description = description;
            UnitPrice = unitPrice;
            MinimumStock = minimumStock;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }
        public void UpdateSku(string sku)
        {
            ValidateSku(sku);
            Sku = sku.Trim();
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateDescription(string description)
        {
            ValidateDescription(description);
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateUnitPrice(decimal unitPrice)
        {
            ValidateUnitPrice(unitPrice);
            UnitPrice = unitPrice;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateMinimumStock(int minimumStock)
        {
            ValidateMinimumStock(minimumStock);
            MinimumStock = minimumStock;
            UpdatedAt = DateTime.UtcNow;
        }
        public void ChangeCategory(int categoryId)
        {
            ValidateCategoryId(categoryId);
            CategoryId = categoryId;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        public void AddStock(int quantity)
        {
            ValidateQuantity(quantity);
            CurrentStock = checked(CurrentStock + quantity);
            UpdatedAt = DateTime.UtcNow;
        }
        public void RemoveStock(int quantity)
        {
            ValidateQuantity(quantity);
            if (quantity > CurrentStock)
                throw new InvalidOperationException("Insufficient stock to complete the operation.");
            CurrentStock -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }
        private static void ValidateCategoryId(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be greater than zero.", nameof(categoryId));
        }
        private static void ValidateSku(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU is required.", nameof(sku));
            if (sku.Trim().Length > 50)
                throw new ArgumentException("SKU cannot exceed 50 characters.", nameof(sku));
        }
        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));
            if (name.Trim().Length > 150)
                throw new ArgumentException("Product name cannot exceed 150 characters.", nameof(name));
        } 
        private static void ValidateDescription(string? description)
        {
            if (description?.Length > 1000)
                throw new ArgumentException("Product description cannot exceed 1000 characters.", nameof(description));
        }
        private static void ValidateUnitPrice(decimal unitPrice)
        {
            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));
        }
        private static void ValidateMinimumStock(int minimumStock)
        {
            if (minimumStock < 0)
                throw new ArgumentException("Minimum stock cannot be negative.", nameof(minimumStock));
        }
        private static void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }
    }
}