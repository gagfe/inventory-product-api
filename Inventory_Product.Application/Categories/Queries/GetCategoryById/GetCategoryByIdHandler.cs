using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Categories.DTOs;
namespace Inventory_Product.Application.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdHandler
    {
        private readonly ICategoryReadRepository _repository;
        public GetCategoryByIdHandler(ICategoryReadRepository repository) => _repository = repository;
        public Task<CategoryResponse?> HandleAsync(int categoryId, CancellationToken cancellationToken)
        {
            if (categoryId <= 0)
                throw new ArgumentException("The category ID must be greater than zero.");
            return _repository.GetByIdAsync(categoryId, cancellationToken);
        }
    }
}