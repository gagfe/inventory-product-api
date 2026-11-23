using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Categories.DTOs;
namespace Inventory_Product.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryHandler
    {
        private readonly ICategoryCommandRepository _repository;
        public CreateCategoryHandler(ICategoryCommandRepository repository) => _repository = repository;
        public Task<int> HandleAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            Validate(request);
            return _repository.CreateAsync(request, cancellationToken);
        }
        private static void Validate(CreateCategoryRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);
            if (request.Name.Length > 100 || request.Description?.Length > 500)
                throw new ArgumentException("The category data is invalid.");
        }
    }
}