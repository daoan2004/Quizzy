using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;

namespace ProjectBase.Tests;

internal sealed class AuthenticatedTestSession : IAsyncDisposable
{
    private static long _nextId = 80000;

    private AuthenticatedTestSession(
        QuizzyWebApplicationFactory factory,
        HttpClient client,
        long userId)
    {
        Factory = factory;
        Client = client;
        UserId = userId;
    }

    public QuizzyWebApplicationFactory Factory { get; }
    public HttpClient Client { get; }
    public long UserId { get; }

    public static async Task<AuthenticatedTestSession> CreateAsync(string roleName = "Customer")
    {
        var factory = new QuizzyWebApplicationFactory();
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var userId = Interlocked.Increment(ref _nextId);
        var roleId = Interlocked.Increment(ref _nextId);
        const string password = "Security@123";
        var email = $"security-{userId}@quizzy.test";

        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Add(new Role { RoleID = roleId, RoleName = roleName });
            context.Users.Add(new User
            {
                ID = userId,
                email = email,
                fullname = "Security Test",
                password = PasswordServiceTests.LegacyMd5(password),
                Phone = "0901234567",
                gender = true,
                RoleID = roleId,
                status = 1
            });
            await context.SaveChangesAsync();
        }

        using var response = await client.PostAsJsonAsync("/Account/Login", new
        {
            email,
            password
        });
        response.EnsureSuccessStatusCode();
        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        if (!payload.RootElement.GetProperty("success").GetBoolean())
        {
            throw new InvalidOperationException("The test account could not log in.");
        }

        return new AuthenticatedTestSession(factory, client, userId);
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
    }
}
