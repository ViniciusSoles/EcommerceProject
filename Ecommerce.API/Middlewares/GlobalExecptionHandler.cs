using ECommerceApi.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = MapException(exception, httpContext);

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails MapException(Exception exception, HttpContext httpContext)
    {
        return exception switch
        {
            // ↓ NOVO — captura qualquer subclasse de DomainException
            DomainException => new ProblemDetails
            {
                Title    = "Business rule violation.",
                Detail   = exception.Message,
                Status   = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path
            },

            ArgumentException => new ProblemDetails
            {
                Title    = "Invalid argument.",
                Detail   = exception.Message,
                Status   = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path
            },

            InvalidOperationException => new ProblemDetails
            {
                Title    = "Invalid operation.",
                Detail   = exception.Message,
                Status   = StatusCodes.Status409Conflict,
                Instance = httpContext.Request.Path
            },

            KeyNotFoundException => new ProblemDetails
            {
                Title    = "Resource not found.",
                Detail   = exception.Message,
                Status   = StatusCodes.Status404NotFound,
                Instance = httpContext.Request.Path
            },

            UnauthorizedAccessException => new ProblemDetails
            {
                Title    = "Unauthorized access.",
                Detail   = exception.Message,
                Status   = StatusCodes.Status401Unauthorized,
                Instance = httpContext.Request.Path
            },

            _ => new ProblemDetails
            {
                Title    = "An unexpected error occurred.",
                Detail   = "Please try again later or contact support.",
                Status   = StatusCodes.Status500InternalServerError,
                Instance = httpContext.Request.Path
            }
        };
    }
}






