using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Categories.DTOs;
namespace Inventory_Product.Application.Categories.Queries.GetCategories
{
    public class GetCategoriesHandler
    {
        private readonly ICategoryReadRepository _repository;
        public GetCategoriesHandler(ICategoryReadRepository repository) => _repository = repository;
        public Task<IReadOnlyList<CategoryResponse>> HandleAsync(CancellationToken cancellationToken) => _repository.GetAllAsync(cancellationToken);
    }
}