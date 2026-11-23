using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Inventory_Product.Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        private Category() { }
        public Category(string name, string? description)
        {
            ValidateName(name);
            ValidateDescription(description);
            Name = name.Trim();
            Description = description;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }
        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name.Trim();
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateDescription(string? description)
        {
            ValidateDescription(description);
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required.", nameof(name));
            if (name.Trim().Length > 100)
                throw new ArgumentException("Category name cannot exceed 100 characters.", nameof(name));
        }
        private static void ValidateDescription(string? description)
        {
            if (description?.Length > 500)
                throw new ArgumentException("Category description cannot exceed 500 characters.", nameof(description));
        }
    }
}