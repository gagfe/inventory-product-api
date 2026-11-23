using Inventory_Product.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Product.Api.Exceptions
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title, detail) = exception switch
            {
                DuplicateResourceException => (
                    StatusCodes.Status409Conflict,
                    "Resource already exists",
                    exception.Message),

                ArgumentException => (
                    StatusCodes.Status400BadRequest,
                    "Invalid request",
                    exception.Message),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Internal server error",
                    "An unexpected error occurred.")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "An unexpected error occurred.");
            }

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["traceId"] =
                httpContext.TraceIdentifier;

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }
    }
}