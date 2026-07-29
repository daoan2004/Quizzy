using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectBase.Middleware;

namespace ProjectBase.Tests;

public sealed class GlobalExceptionHandlingTests
{
    [Fact]
    public async Task Api_exception_returns_safe_problem_details_with_trace_id()
    {
        var middleware = CreateMiddleware();
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "qa-trace-id"
        };
        context.Request.Path = "/api/PracticeApi/test";
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Position = 0;
        using var body = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("An unexpected error occurred.", body.RootElement.GetProperty("title").GetString());
        Assert.Equal("qa-trace-id", body.RootElement.GetProperty("traceId").GetString());
        Assert.DoesNotContain("sensitive failure detail", body.RootElement.GetRawText());
    }

    [Fact]
    public async Task Page_exception_is_rethrown_for_the_outer_html_exception_handler()
    {
        var middleware = CreateMiddleware();
        var context = new DefaultHttpContext();
        context.Request.Path = "/Subjects";

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => middleware.InvokeAsync(context));

        Assert.Equal("sensitive failure detail", exception.Message);
    }

    private static ApiExceptionHandlingMiddleware CreateMiddleware() =>
        new(
            _ => throw new InvalidOperationException("sensitive failure detail"),
            NullLogger<ApiExceptionHandlingMiddleware>.Instance);
}
