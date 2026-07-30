using System.Net;

namespace ProjectBase.Tests;

public sealed class SecurityHeadersTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;

    public SecurityHeadersTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/health/live")]
    [InlineData("/api/PracticeApi/LoadFilter/1")]
    public async Task Security_headers_are_applied_to_html_health_and_api_responses(
        string path)
    {
        using var client = _factory.CreateClient();

        using var response = await client.GetAsync(path);

        Assert.Equal("nosniff", Header(response, "X-Content-Type-Options"));
        Assert.Equal("DENY", Header(response, "X-Frame-Options"));
        Assert.Equal(
            "strict-origin-when-cross-origin",
            Header(response, "Referrer-Policy"));
        Assert.Contains(
            "frame-ancestors 'none'",
            Header(response, "Content-Security-Policy"));
        Assert.Contains(
            "payment=()",
            Header(response, "Permissions-Policy"));
    }

    [Fact]
    public async Task Protected_api_still_returns_unauthorized_with_security_headers()
    {
        using var client = _factory.CreateClient();

        using var response = await client.GetAsync(
            "/api/PracticeApi/LoadFilter/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("nosniff", Header(response, "X-Content-Type-Options"));
    }

    private static string Header(
        HttpResponseMessage response,
        string name) =>
        Assert.Single(response.Headers.GetValues(name));
}
