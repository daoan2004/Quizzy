using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectBase.Helpers;

namespace ProjectBase.Tests;

public sealed class MigrationStrategyTests
{
    private static readonly string SolutionRoot = FindSolutionRoot();

    [Fact]
    public void Application_startup_does_not_apply_migrations_implicitly()
    {
        var program = File.ReadAllText(
            Path.Combine(SolutionRoot, "ProjectBase", "Program.cs"));

        Assert.DoesNotContain(".Database.Migrate(", program);
        Assert.DoesNotContain(".Database.EnsureCreated(", program);
    }

    [Fact]
    public void Migration_script_supports_status_idempotent_script_and_explicit_apply()
    {
        var script = File.ReadAllText(
            Path.Combine(
                SolutionRoot,
                "ProjectBase",
                "scripts",
                "database-migrate.ps1"));

        Assert.Contains("[ValidateSet(\"Status\", \"Script\", \"Apply\")]", script);
        Assert.Contains("migrations list", script);
        Assert.Contains("migrations script", script);
        Assert.Contains("--idempotent", script);
        Assert.Contains("database update", script);
    }

    [Fact]
    public async Task Readiness_is_unhealthy_when_database_has_pending_migrations()
    {
        await using var factory = new PendingMigrationFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health/ready");
        using var body = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(
            "Unhealthy",
            body.RootElement
                .GetProperty("checks")
                .GetProperty("database")
                .GetProperty("status")
                .GetString());
        Assert.Contains(
            "migration(s) are pending",
            body.RootElement
                .GetProperty("checks")
                .GetProperty("database")
                .GetProperty("description")
                .GetString());
    }

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "SWP391.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the solution root.");
    }

    private sealed class PendingMigrationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DataContext>();
                services.RemoveAll<DbContextOptions<DataContext>>();
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlite(
                        $"Data Source=Pending-{Guid.NewGuid()};Mode=Memory;Cache=Shared"));
            });
        }
    }
}
