using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;
using ProjectBase.Services;

namespace ProjectBase.Tests;

public sealed class PasswordFlowIntegrationTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PasswordFlowIntegrationTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_stores_modern_password_hash()
    {
        await _factory.ResetDatabaseAsync();

        using var response = await _client.PostAsJsonAsync("/Account/Register", new
        {
            fullname = "Hash Test",
            password = "Customer@123",
            confirmPassword = "Customer@123",
            email = "hash-register@quizzy.test",
            phone = "0901234567",
            gender = true
        });

        Assert.True(await ReadSuccessAsync(response));

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        var user = Assert.Single(context.Users);

        Assert.True(passwordService.VerifyPassword(user, "Customer@123").Succeeded);
        Assert.DoesNotMatch("^[0-9a-fA-F]{32}$", user.password);
    }

    [Fact]
    public async Task Legacy_md5_login_migrates_hash()
    {
        await _factory.ResetDatabaseAsync();
        const string password = "Legacy@123";
        var userId = await AddActiveUserAsync(
            "legacy-login@quizzy.test",
            PasswordServiceTests.LegacyMd5(password));

        using var response = await _client.PostAsJsonAsync("/Account/Login", new
        {
            email = "legacy-login@quizzy.test",
            password
        });

        Assert.True(await ReadSuccessAsync(response));

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        var migratedUser = await context.Users.FindAsync(userId);

        Assert.NotNull(migratedUser);
        Assert.DoesNotMatch("^[0-9a-fA-F]{32}$", migratedUser.password);
        Assert.True(passwordService.VerifyPassword(migratedUser, password).Succeeded);
    }

    [Fact]
    public async Task Wrong_password_is_rejected()
    {
        await _factory.ResetDatabaseAsync();
        await AddActiveUserAsync(
            "wrong-password@quizzy.test",
            PasswordServiceTests.LegacyMd5("Correct@123"));

        using var response = await _client.PostAsJsonAsync("/Account/Login", new
        {
            email = "wrong-password@quizzy.test",
            password = "Wrong@123"
        });

        Assert.False(await ReadSuccessAsync(response));
    }

    [Fact]
    public async Task Modern_password_hash_can_log_in()
    {
        await _factory.ResetDatabaseAsync();
        string passwordHash;

        using (var scope = _factory.Services.CreateScope())
        {
            var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
            var user = new User
            {
                email = "modern-login@quizzy.test",
                fullname = "Modern Login",
                password = string.Empty,
                Phone = "0901234567"
            };
            passwordHash = passwordService.HashPassword(user, "Modern@123");
        }

        await AddActiveUserAsync("modern-login@quizzy.test", passwordHash);

        using var response = await _client.PostAsJsonAsync("/Account/Login", new
        {
            email = "modern-login@quizzy.test",
            password = "Modern@123"
        });

        Assert.True(await ReadSuccessAsync(response));
    }

    [Fact]
    public async Task Reset_password_stores_modern_hash_and_clears_token()
    {
        await _factory.ResetDatabaseAsync();
        const string token = "valid-reset-token";
        var userId = await AddActiveUserAsync(
            "reset-hash@quizzy.test",
            PasswordServiceTests.LegacyMd5("OldPassword@123"),
            token);

        using var response = await _client.PostAsJsonAsync("/Account/ResetPasswordConfirm", new
        {
            newPassword = "NewPassword@123",
            reNewPassword = "NewPassword@123",
            token
        });

        Assert.True(await ReadSuccessAsync(response));

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        var user = await context.Users.FindAsync(userId);

        Assert.NotNull(user);
        Assert.True(passwordService.VerifyPassword(user, "NewPassword@123").Succeeded);
        Assert.Null(user.PasswordResetToken);
        Assert.Null(user.PasswordResetTokenExpires);
    }

    [Fact]
    public async Task Change_password_verifies_current_and_stores_modern_hash()
    {
        await _factory.ResetDatabaseAsync();
        var userId = await AddActiveUserAsync(
            "change-password@quizzy.test",
            PasswordServiceTests.LegacyMd5("Current@123"));

        using var loginResponse = await _client.PostAsJsonAsync("/Account/Login", new
        {
            email = "change-password@quizzy.test",
            password = "Current@123"
        });
        Assert.True(await ReadSuccessAsync(loginResponse));

        using var changeResponse = await _client.PostAsJsonAsync("/Account/ChangePassword", new
        {
            currentPassword = "Current@123",
            newPassword = "Changed@123",
            confirmNewPassword = "Changed@123"
        });
        Assert.True(await ReadSuccessAsync(changeResponse));

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        var user = await context.Users.FindAsync(userId);

        Assert.NotNull(user);
        Assert.False(passwordService.VerifyPassword(user, "Current@123").Succeeded);
        Assert.True(passwordService.VerifyPassword(user, "Changed@123").Succeeded);
        Assert.DoesNotMatch("^[0-9a-fA-F]{32}$", user.password);
    }

    private async Task<long> AddActiveUserAsync(
        string email,
        string passwordHash,
        string? resetToken = null)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var user = new User
        {
            ID = 60001,
            email = email,
            fullname = "Password Flow Test",
            password = passwordHash,
            Phone = "0901234567",
            gender = true,
            RoleID = 2,
            status = 1,
            PasswordResetToken = resetToken,
            PasswordResetTokenExpires = resetToken is null
                ? null
                : TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).AddHours(1)
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.ID!.Value;
    }

    private static async Task<bool> ReadSuccessAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return payload.RootElement.GetProperty("success").GetBoolean();
    }
}
