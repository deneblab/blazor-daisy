using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Deneblab.BlazorDaisy.Infrastructure.Middleware;

/// <summary>
/// Global error handling middleware for catching unhandled exceptions.
/// Returns ProblemDetails for API requests, redirects for page requests.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            // Handle status codes for non-started responses
            if (!context.Response.HasStarted)
            {
                await HandleStatusCodeAsync(context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleStatusCodeAsync(HttpContext context)
    {
        var isApiRequest = context.Request.Path.StartsWithSegments("/api");

        switch (context.Response.StatusCode)
        {
            case (int)HttpStatusCode.NotFound:
                if (isApiRequest)
                {
                    await WriteProblemDetailsAsync(context, HttpStatusCode.NotFound,
                        "Not Found", "The requested resource was not found.");
                }
                else
                {
                    context.Request.Path = "/not-found";
                    await _next(context);
                }
                break;

            case (int)HttpStatusCode.Forbidden:
                if (isApiRequest)
                {
                    await WriteProblemDetailsAsync(context, HttpStatusCode.Forbidden,
                        "Forbidden", "You do not have permission to access this resource.");
                }
                else
                {
                    context.Request.Path = "/access-denied";
                    await _next(context);
                }
                break;

            case (int)HttpStatusCode.Unauthorized:
                if (isApiRequest)
                {
                    await WriteProblemDetailsAsync(context, HttpStatusCode.Unauthorized,
                        "Unauthorized", "Authentication is required to access this resource.");
                }
                break;
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var isApiRequest = context.Request.Path.StartsWithSegments("/api");

        if (isApiRequest)
        {
            var (statusCode, title, detail) = exception switch
            {
                ArgumentException => (HttpStatusCode.BadRequest, "Bad Request", exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", "Authentication required."),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Not Found", exception.Message),
                _ => (HttpStatusCode.InternalServerError, "Internal Server Error",
                    _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.")
            };

            await WriteProblemDetailsAsync(context, statusCode, title, detail, exception);
        }
        else
        {
            context.Response.Redirect("/Error");
        }
    }

    private async Task WriteProblemDetailsAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string title,
        string detail,
        Exception? exception = null)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        // Add trace ID for correlation
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        // Add stack trace in development
        if (_env.IsDevelopment() && exception != null)
        {
            problemDetails.Extensions["exception"] = exception.ToString();
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}

/// <summary>
/// Extension methods for error handling middleware.
/// </summary>
public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
