using Inventory_Product.Application.Categories.Commands.CreateCategory;
using Inventory_Product.Application.Categories.Commands.DeactivateCategory;
using Inventory_Product.Application.Categories.Commands.UpdateCategory;
using Inventory_Product.Application.Categories.DTOs;
using Inventory_Product.Application.Categories.Queries.GetCategories;
using Inventory_Product.Application.Categories.Queries.GetCategoryById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Product.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly GetCategoriesHandler _getCategoriesHandler;
        private readonly GetCategoryByIdHandler _getCategoryByIdHandler;
        private readonly CreateCategoryHandler _createCategoryHandler;
        private readonly UpdateCategoryHandler _updateCategoryHandler;
        private readonly DeactivateCategoryHandler _deactivateCategoryHandler;

        public CategoriesController(GetCategoriesHandler getCategoriesHandler, GetCategoryByIdHandler getCategoryByIdHandler, CreateCategoryHandler createCategoryHandler, UpdateCategoryHandler updateCategoryHandler, DeactivateCategoryHandler deactivateCategoryHandler)
        {
            _getCategoriesHandler = getCategoriesHandler;
            _getCategoryByIdHandler = getCategoryByIdHandler;
            _createCategoryHandler = createCategoryHandler;
            _updateCategoryHandler = updateCategoryHandler;
            _deactivateCategoryHandler = deactivateCategoryHandler;
        }

        [HttpGet]
        [Authorize(Policy = "InventoryAccess")]
        public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(
            CancellationToken cancellationToken)
        {
            var categories = await _getCategoriesHandler.HandleAsync(cancellationToken);
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "InventoryAccess")]
        public async Task<ActionResult<CategoryResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var category = await _getCategoryByIdHandler.HandleAsync(id, cancellationToken);
            return category is null
                ? Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Category not found",
                    detail: "The category does not exist or is inactive.")
                : Ok(category);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var categoryId = await _createCategoryHandler.HandleAsync(request, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = categoryId },
                new { categoryId });
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var updated = await _updateCategoryHandler.HandleAsync(id, request, cancellationToken);
            return updated
                ? NoContent()
                : Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Category not found",
                    detail: "The category does not exist or is inactive.");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
        {
            var deactivated = await _deactivateCategoryHandler.HandleAsync(id, cancellationToken);
            return deactivated
                ? NoContent()
                : Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Category cannot be deactivated",
                    detail: "The category does not exist, is inactive, or still has active products.");
        }
    }
}