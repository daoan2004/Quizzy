using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectBase.Configuration;
using ProjectBase.Helpers;

namespace ProjectBase.Tests;

public sealed class ProductionConfigurationValidatorTests
{
    [Fact]
    public void Missing_required_values_are_reported_by_key_without_secret_values()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["Email:Password"] = "super-secret-value",
            ["Email:Port"] = "587",
            ["PasswordResetLinkExpirationHours"] = "1",
            ["VerificationLinkExpirationHours"] = "24"
        });

        var exception = Assert.Throws<InvalidOperationException>(
            () => ProductionConfigurationValidator.Validate(configuration));

        Assert.Contains("ConnectionStrings:ConnectedDb is required.", exception.Message);
        Assert.Contains("Email:FromAddress is required.", exception.Message);
        Assert.Contains("Email:BaseUrl is required.", exception.Message);
        Assert.DoesNotContain("super-secret-value", exception.Message);
    }

    [Fact]
    public void Complete_production_configuration_is_accepted()
    {
        var configuration = BuildConfiguration(ValidSettings());

        var errors = ProductionConfigurationValidator.GetErrors(configuration);

        Assert.Empty(errors);
        ProductionConfigurationValidator.Validate(configuration);
    }

    [Fact]
    public void Invalid_url_port_and_expiration_are_rejected()
    {
        var settings = ValidSettings();
        settings["Email:BaseUrl"] = "localhost-only";
        settings["Email:Port"] = "70000";
        settings["VerificationLinkExpirationHours"] = "0";

        var errors = ProductionConfigurationValidator.GetErrors(
            BuildConfiguration(settings));

        Assert.Contains(errors, error => error.StartsWith("Email:BaseUrl"));
        Assert.Contains(errors, error => error.StartsWith("Email:Port"));
        Assert.Contains(errors, error => error.StartsWith("VerificationLinkExpirationHours"));
    }

    [Fact]
    public async Task Complete_configuration_starts_the_production_pipeline()
    {
        const string fromAddressKey = "Email__FromAddress";
        const string passwordKey = "Email__Password";
        var previousFromAddress = Environment.GetEnvironmentVariable(fromAddressKey);
        var previousPassword = Environment.GetEnvironmentVariable(passwordKey);

        try
        {
            Environment.SetEnvironmentVariable(fromAddressKey, "no-reply@quizzy.test");
            Environment.SetEnvironmentVariable(passwordKey, "test-placeholder");

            await using var factory = new ProductionWebApplicationFactory();
            using var client = factory.CreateClient();
            using var response = await client.GetAsync("/health/ready");

            response.EnsureSuccessStatusCode();
            Assert.Contains(
                "\"status\":\"Healthy\"",
                await response.Content.ReadAsStringAsync());
        }
        finally
        {
            Environment.SetEnvironmentVariable(fromAddressKey, previousFromAddress);
            Environment.SetEnvironmentVariable(passwordKey, previousPassword);
        }
    }

    private static Dictionary<string, string?> ValidSettings() => new()
    {
        ["ConnectionStrings:ConnectedDb"] =
            "Server=database;Database=Quizzy;User Id=quizzy;Password=placeholder;",
        ["Email:FromAddress"] = "no-reply@quizzy.test",
        ["Email:Host"] = "smtp.quizzy.test",
        ["Email:Port"] = "587",
        ["Email:Password"] = "placeholder",
        ["Email:BaseUrl"] = "https://quizzy.test/",
        ["PasswordResetLinkExpirationHours"] = "1",
        ["VerificationLinkExpirationHours"] = "24"
    };

    private static IConfiguration BuildConfiguration(
        IDictionary<string, string?> values) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

    private sealed class ProductionWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Production");
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(ValidSettings()));
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DataContext>();
                services.RemoveAll<DbContextOptions<DataContext>>();
                services.AddDbContext<DataContext>(options =>
                    options.UseInMemoryDatabase($"ProductionSmoke-{Guid.NewGuid()}"));
            });
        }
    }
}
