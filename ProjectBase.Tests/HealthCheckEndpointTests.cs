using System.Net;
using System.Text.Json;

namespace ProjectBase.Tests;

public sealed class HealthCheckEndpointTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;

    public HealthCheckEndpointTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Live_endpoint_reports_only_process_health()
    {
        using var client = _factory.CreateClient();

        using var response = await client.GetAsync("/health/live");
        using var body = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("Healthy", body.RootElement.GetProperty("status").GetString());
        Assert.True(body.RootElement.GetProperty("checks").TryGetProperty("self", out _));
        Assert.False(body.RootElement.GetProperty("checks").TryGetProperty("database", out _));
    }

    [Fact]
    public async Task Ready_endpoint_includes_database_health()
    {
        using var client = _factory.CreateClient();

        using var response = await client.GetAsync("/health/ready");
        using var body = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", body.RootElement.GetProperty("status").GetString());
        Assert.Equal(
            "Healthy",
            body.RootElement
                .GetProperty("checks")
                .GetProperty("database")
                .GetProperty("status")
                .GetString());
    }
}
