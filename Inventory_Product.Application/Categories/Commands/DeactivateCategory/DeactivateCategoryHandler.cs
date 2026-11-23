using Inventory_Product.Application.Abstraction.Persistence;
namespace Inventory_Product.Application.Categories.Commands.DeactivateCategory
{
    public class DeactivateCategoryHandler
    {
        private readonly ICategoryCommandRepository _repository;
        public DeactivateCategoryHandler(ICategoryCommandRepository repository) => _repository = repository;
        public Task<bool> HandleAsync(int categoryId, CancellationToken cancellationToken)
        {
            if (categoryId <= 0)
                throw new ArgumentException("The category ID must be greater than zero.");
            return _repository.DeactivateAsync(categoryId, cancellationToken);
        }
    }
}