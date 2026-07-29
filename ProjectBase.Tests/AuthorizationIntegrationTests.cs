using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;

namespace ProjectBase.Tests;

public sealed class AuthorizationIntegrationTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;

    public AuthorizationIntegrationTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Guest_CanAccessPublicHomePage()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Guest_IsRedirectedToLogin_FromDashboardPage()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Dashboard");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/Account/Login", response.Headers.Location?.AbsolutePath);
    }

    [Fact]
    public async Task Guest_GetsUnauthorized_FromPrivateDashboardApi()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/api/dashboardapi/registrations");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Null(response.Headers.Location);
    }

    [Fact]
    public void AuthenticationCookie_UsesExpectedSecuritySettings()
    {
        var options = _factory.Services
            .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
            .Get(CookieAuthenticationDefaults.AuthenticationScheme);

        Assert.True(options.Cookie.HttpOnly);
        Assert.Equal(CookieSecurePolicy.SameAsRequest, options.Cookie.SecurePolicy);
        Assert.Equal(SameSiteMode.Lax, options.Cookie.SameSite);
        Assert.Equal(TimeSpan.FromMinutes(60), options.ExpireTimeSpan);
        Assert.True(options.SlidingExpiration);
    }

    [Theory]
    [InlineData("Marketing")]
    [InlineData("Admin")]
    public async Task AuthorizedDashboardRole_CanAccessDashboard(string roleName)
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        await AddUserAndLoginAsync(factory, client, roleName);

        var response = await client.GetAsync("/Dashboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Customer_GetsForbidden_FromDashboardApi()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        await AddUserAndLoginAsync(factory, client, "Customer");

        var response = await client.GetAsync("/api/dashboardapi/registrations");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Null(response.Headers.Location);
    }

    [Fact]
    public async Task Logout_ClearsAuthenticationSession()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        await AddUserAndLoginAsync(factory, client, "Marketing");
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/Dashboard")).StatusCode);

        var logoutResponse = await client.GetAsync("/Account/Logout");
        var dashboardResponse = await client.GetAsync("/Dashboard");

        Assert.Equal(HttpStatusCode.Redirect, logoutResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, dashboardResponse.StatusCode);
        Assert.Equal("/Account/Login", dashboardResponse.Headers.Location?.AbsolutePath);
    }

    private static async Task AddUserAndLoginAsync(
        QuizzyWebApplicationFactory factory,
        HttpClient client,
        string roleName)
    {
        const string password = "RoleTest@123";
        var email = $"{roleName.ToLowerInvariant()}-{Guid.NewGuid():N}@quizzy.test";

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var role = new Role
            {
                RoleID = 70001,
                RoleName = roleName
            };
            context.Add(role);
            context.Users.Add(new User
            {
                ID = 70001,
                email = email,
                fullname = $"{roleName} Test",
                password = PasswordServiceTests.LegacyMd5(password),
                Phone = "0901234567",
                gender = true,
                RoleID = role.RoleID,
                status = 1
            });
            await context.SaveChangesAsync();
        }

        using var loginResponse = await client.PostAsJsonWithCsrfAsync("/Account/Login", new
        {
            email,
            password
        });
        loginResponse.EnsureSuccessStatusCode();
        using var payload = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
        Assert.True(payload.RootElement.GetProperty("success").GetBoolean());
    }
}
