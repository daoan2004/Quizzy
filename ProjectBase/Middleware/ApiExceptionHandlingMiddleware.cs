using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ProjectBase.Middleware;

public sealed class ApiExceptionHandlingMiddleware
{
    private const string ApiPathPrefix = "/api";
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

    public ApiExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception) when (context.Request.Path.StartsWithSegments(ApiPathPrefix))
        {
            _logger.LogError(
                exception,
                "Unhandled API exception for {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = "Please try again later. Use the trace ID when contacting support.",
                Instance = context.Request.Path
            };
            problem.Extensions["traceId"] = context.TraceIdentifier;

            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                problem,
                problem.GetType(),
                cancellationToken: context.RequestAborted);
        }
    }
}
