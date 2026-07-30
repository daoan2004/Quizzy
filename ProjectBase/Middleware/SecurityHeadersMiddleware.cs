namespace ProjectBase.Middleware;

public sealed class SecurityHeadersMiddleware
{
    private const string ContentSecurityPolicy =
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "frame-ancestors 'none'; " +
        "object-src 'none'";

    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Content-Security-Policy"] = ContentSecurityPolicy;
            headers["Permissions-Policy"] =
                "camera=(), microphone=(), geolocation=(), payment=()";
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
