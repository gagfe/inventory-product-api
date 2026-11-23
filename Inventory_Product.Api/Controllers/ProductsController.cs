using Inventory_Product.Application.Products.Commands.CreateProduct;
using Inventory_Product.Application.Products.Commands.DeactivateProduct;
using Inventory_Product.Application.Products.Commands.UpdateProduct;
using Inventory_Product.Application.Products.DTOs;
using Inventory_Product.Application.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Product.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly GetProductsHandler _getProductsHandler;
        private readonly CreateProductHandler _createProductHandler;
        private readonly UpdateProductHandler _updateProductHandler;
        private readonly DeactivateProductHandler _deactivateProductHandler;
        public ProductsController(GetProductsHandler getProductsHandler, CreateProductHandler createProductHandler, UpdateProductHandler updateProductHandler, DeactivateProductHandler deactivateProductHandler)
        {
            _getProductsHandler = getProductsHandler;
            _createProductHandler = createProductHandler;
            _updateProductHandler = updateProductHandler;
            _deactivateProductHandler = deactivateProductHandler;
        }

        [HttpGet]
        [Authorize(Policy = "InventoryAccess")]
        public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var products = await _getProductsHandler.HandleAsync(cancellationToken);
            return Ok(products);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var productId = await _createProductHandler.HandleAsync(request, cancellationToken);
            if (productId is null)
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid category", detail: "The category does not exist or is inactive.");
            return Created("/api/products", new { productId });
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _updateProductHandler.HandleAsync(id, request, cancellationToken);
            return result switch
            {
                UpdateProductResult.Updated => NoContent(),
                UpdateProductResult.NotFound => Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found", detail: "The product does not exist or is inactive."),
                UpdateProductResult.InvalidCategory => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid category", detail: "The category does not exist or is inactive."),
                UpdateProductResult.ConcurrencyConflict => Problem(statusCode: StatusCodes.Status409Conflict, title: "Concurrency conflict", detail: "The product was modified by another operation. Retrieve the latest version and try again."),

                _ => Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal server error",
                    detail: "An unexpected update result occurred.")
            };
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
        {
            var deactivated = await _deactivateProductHandler.HandleAsync(id, cancellationToken);
            return deactivated
                ? NoContent() : Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found", detail: "The product does not exist or is already inactive.");
        }
    }
}