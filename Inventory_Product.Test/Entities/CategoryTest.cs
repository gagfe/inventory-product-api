using Inventory_Product.Domain.Entities;

namespace Inventory_Product.Test.Entities
{
    public class CategoryTest
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateActiveCategory()
        {
            var category = new Category("Electronics", "Electronic devices");
            Assert.Equal("Electronics", category.Name);
            Assert.Equal("Electronic devices", category.Description);
            Assert.True(category.IsActive);
            Assert.NotEqual(default, category.CreatedAt);
            Assert.Null(category.UpdatedAt);
        }
        [Fact]
        public void Constructor_WithNullDescription_ShouldCreateCategory()
        {
            var category = new Category("Electronics", null);
            Assert.Null(category.Description);
        }
        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Category("", null));
        }
        [Fact]
        public void Constructor_WithNameExceeding100Characters_ShouldThrowArgumentException()
        {
            var name = new string('A', 101);
            Assert.Throws<ArgumentException>(() => new Category(name, null));
        }
        [Fact]
        public void Constructor_WithDescriptionExceeding500Characters_ShouldThrowArgumentException()
        {
            var description = new string('A', 501);
            Assert.Throws<ArgumentException>(() => new Category("Electronics", description));
        }
        [Fact]
        public void UpdateName_WithValidName_ShouldUpdateNameAndUpdatedAt()
        {
            var category = new Category("Electronics", null);
            category.UpdateName("Office Supplies");
            Assert.Equal("Office Supplies", category.Name);
            Assert.NotNull(category.UpdatedAt);
        }
        [Fact]
        public void UpdateName_WithWhitespace_ShouldThrowArgumentException()
        {
            var category = new Category("Electronics", null);
            Assert.Throws<ArgumentException>(() => category.UpdateName("   "));
        }
        [Fact]
        public void UpdateDescription_WithNull_ShouldAllowNullDescription()
        {
            var category = new Category("Electronics", "Electronic devices");
            category.UpdateDescription(null);
            Assert.Null(category.Description);
        }
        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            var category = new Category("Electronics", null);
            category.Deactivate();
            Assert.False(category.IsActive);
            Assert.NotNull(category.UpdatedAt);
        }
        [Fact]
        public void Activate_ShouldSetIsActiveToTrue()
        {
            var category = new Category("Electronics", null);
            category.Deactivate();
            category.Activate();
            Assert.True(category.IsActive);
        }
    }
}
