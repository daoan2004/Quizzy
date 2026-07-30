using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;

namespace ProjectBase.Tests;

public sealed class AccountRegistrationValidationIntegrationTests
{
    [Fact]
    public async Task Duplicate_email_is_rejected_without_creating_another_user()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient();
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Users.Add(new User
            {
                ID = 97001,
                email = "duplicate@quizzy.test",
                fullname = "Existing User",
                password = PasswordServiceTests.ModernHash("Password@123"),
                Phone = "0901234567",
                gender = true,
                RoleID = 2,
                status = 1
            });
            await context.SaveChangesAsync();
        }

        using var response = await client.PostAsJsonWithCsrfAsync(
            "/Account/Register",
            ValidRegistration("duplicate@quizzy.test", "Customer@123"));
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.False(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Contains(
            json.RootElement.GetProperty("errors").EnumerateArray(),
            error => error.GetString()!.Contains("already in use", StringComparison.OrdinalIgnoreCase));
        using var verificationScope = factory.Services.CreateScope();
        Assert.Single(
            verificationScope.ServiceProvider.GetRequiredService<DataContext>().Users);
    }

    [Fact]
    public async Task Weak_password_is_rejected_without_creating_user()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonWithCsrfAsync(
            "/Account/Register",
            ValidRegistration("weak-password@quizzy.test", "weak"));
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.False(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Contains(
            json.RootElement.GetProperty("errors").EnumerateArray(),
            error => error.GetString()!.Contains("at least 8", StringComparison.OrdinalIgnoreCase));
        using var scope = factory.Services.CreateScope();
        Assert.Empty(scope.ServiceProvider.GetRequiredService<DataContext>().Users);
    }

    private static object ValidRegistration(string email, string password) => new
    {
        fullname = "Registration Test",
        password,
        confirmPassword = password,
        email,
        phone = "0901234567",
        gender = true
    };
}
