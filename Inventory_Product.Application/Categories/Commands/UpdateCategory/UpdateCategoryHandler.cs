using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Categories.DTOs;
namespace Inventory_Product.Application.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryHandler
    {
        private readonly ICategoryCommandRepository _repository;
        public UpdateCategoryHandler(ICategoryCommandRepository repository) => _repository = repository;
        public Task<bool> HandleAsync(int categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            Validate(categoryId, request);
            return _repository.UpdateAsync(categoryId, request, cancellationToken);
        }
        private static void Validate(int categoryId, UpdateCategoryRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);
            if (categoryId <= 0 || request.Name.Length > 100 || request.Description?.Length > 500)
                throw new ArgumentException("The category data is invalid.");
        }
    }
}