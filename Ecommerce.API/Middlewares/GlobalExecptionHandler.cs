using ECommerceApi.Domain.Exceptions;
using EcommerceProject;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Middlewares;

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
        var problemDetails = MapException(exception, httpContext);

        LogException(exception, problemDetails, httpContext);

        httpContext.Response.StatusCode = problemDetails.Status
            ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails MapException(Exception exception, HttpContext httpContext)
    {
        return exception switch
        {
            
            InsufficientStockException domainEx => new ProblemDetails
            {
                Title = "Stock unavailable.",
                Detail = "One or more items are not available in the requested quantity.",
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["errorCode"] = domainEx.ErrorCode
                }
            },

            InvalidOrderStatusTransitionException domainEx => new ProblemDetails
            {
                Title = "Invalid order operation.",
                Detail = "This order cannot be updated to the requested status.",
                Status = StatusCodes.Status409Conflict,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["errorCode"] = domainEx.ErrorCode
                }

            },

            CannotCancelShippedOrderException domainEx => new ProblemDetails
            {
                Title = "Order cannot be cancelled.",
                Detail = "Orders that have already been shipped or delivered cannot be cancelled.",
                Status = StatusCodes.Status409Conflict,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["errorCode"] = domainEx.ErrorCode
                }

            },

            InvalidPaymentStatusException domainEx => new ProblemDetails
            {
                Title = "Invalid payment operation.",
                Detail = "This payment operation is not allowed in the current status.",
                Status = StatusCodes.Status409Conflict,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["errorCode"] = domainEx.ErrorCode
                }

            },

            DomainException domainEx => new ProblemDetails
            {
                Title = "Business rule violation.",
                Detail = "The requested operation violates a business rule.",
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path
               
            },
            

            ArgumentException argEx => new ProblemDetails
            {
                Title = "Invalid argument.",
                Detail = argEx.Message,
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path,

            },  

            _ => new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Detail = "Please try again later or contact support.",
                Status = StatusCodes.Status500InternalServerError,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["traceId"] = httpContext.TraceIdentifier
                   
                }
            }
        };
    }

    private void LogException(
        Exception exception,
        ProblemDetails problemDetails,
        HttpContext httpContext)
    {


        if (exception is DomainException)
        {
            _logger.LogWarning(
                "Domain exception {ExceptionType} on {Method} {Path}: {Message}",
                exception.GetType().Name,
                httpContext.Request.Method,
                httpContext.Request.Path,
                exception.Message);

            return;
        }

       
        var traceId = problemDetails.Extensions.TryGetValue("traceId", out var trace)
            ? trace?.ToString()
            : httpContext.TraceIdentifier;

        _logger.LogError(exception,
            "[[{TraceId}] Unhandled exception on {Method} {Path}: {Message}",
            traceId,
            httpContext.Request.Method,
            httpContext.Request.Path,
            exception.Message);
    }
}