namespace Inventory_Product.Application.Exceptions
{
    public sealed class DuplicateResourceException : Exception
    {
        public DuplicateResourceException(string message) : base(message)
        {
        }
    }
}