using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectBase.Helpers;

namespace ProjectBase.Tests;

public sealed class DatabaseUnavailableTests
{
    [Fact]
    public async Task Live_stays_healthy_while_ready_reports_unavailable_database()
    {
        await using var factory = new UnavailableDatabaseFactory();
        using var client = factory.CreateClient();

        using var liveResponse = await client.GetAsync("/health/live");
        using var readyResponse = await client.GetAsync("/health/ready");
        using var readyBody = await JsonDocument.ParseAsync(
            await readyResponse.Content.ReadAsStreamAsync());

        Assert.Equal(HttpStatusCode.OK, liveResponse.StatusCode);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, readyResponse.StatusCode);
        Assert.Equal(
            "Unhealthy",
            readyBody.RootElement.GetProperty("status").GetString());
        Assert.Equal(
            "Unhealthy",
            readyBody.RootElement
                .GetProperty("checks")
                .GetProperty("database")
                .GetProperty("status")
                .GetString());
    }

    [Fact]
    public async Task Database_failure_returns_safe_error_page_without_connection_details()
    {
        await using var factory = new UnavailableDatabaseFactory();
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        using var response = await client.GetAsync(
            "/Account/VerifyAccount?token=unavailable-database-test");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.DoesNotContain("127.0.0.1,1", body);
        Assert.DoesNotContain("QuizzyUnavailable", body);
    }

    private sealed class UnavailableDatabaseFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DataContext>();
                services.RemoveAll<DbContextOptions<DataContext>>();
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(
                        "Server=127.0.0.1,1;" +
                        "Database=QuizzyUnavailable;" +
                        "Integrated Security=True;" +
                        "Encrypt=False;" +
                        "Connect Timeout=1;" +
                        "ConnectRetryCount=0;"));
            });
        }
    }
}
