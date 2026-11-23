
using Inventory_Product.Application.InventoryMovements.Commands.RegisterInventoryMovement;
using Inventory_Product.Application.InventoryMovements.DTOs;
using Inventory_Product.Application.InventoryMovements.Queries.GetProductMovements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Product.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryMovementsController : ControllerBase
    {
        private readonly RegisterInventoryMovementHandler _registerHandler;
        private readonly GetProductMovementsHandler _getMovementsHandler;
        public InventoryMovementsController(RegisterInventoryMovementHandler registerHandler, GetProductMovementsHandler getMovementsHandler)
        {
            _registerHandler = registerHandler;
            _getMovementsHandler = getMovementsHandler;
        }

        [HttpPost]
        [Authorize(Policy = "InventoryAccess")]
        public async Task<IActionResult> Register([FromBody] RegisterInventoryMovementRequest request, CancellationToken cancellationToken)
        {
            var createdBy = User.FindFirst("preferred_username")?.Value;
            if (string.IsNullOrWhiteSpace(createdBy))
                return Forbid();

            var result = await _registerHandler.HandleAsync(request, createdBy, cancellationToken);
            return result switch
            {
                RegisterInventoryMovementResult.Success => StatusCode(StatusCodes.Status201Created),
                RegisterInventoryMovementResult.ProductNotFound => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Product not found",
                    detail: "The product does not exist or is inactive."),
                RegisterInventoryMovementResult.MovementTypeNotFound => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid movement type",
                    detail: "The movement type does not exist."),
                RegisterInventoryMovementResult.InsufficientStock => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Insufficient stock",
                    detail: "The product does not have enough stock."),
                _ => Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal server error",
                    detail: "An unexpected error occurred.")
            };
        }

        [HttpGet("products/{productId:int}")]
        [Authorize(Policy = "InventoryAccess")]
        public async Task<ActionResult<IReadOnlyList<InventoryMovementResponse>>> GetByProduct(
            int productId,
            CancellationToken cancellationToken)
        {
            var movements = await _getMovementsHandler.HandleAsync(productId, cancellationToken);
            return Ok(movements);
        }
    }
}