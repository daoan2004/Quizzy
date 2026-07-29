using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;

namespace ProjectBase.Tests;

public sealed class AccountTokenSecurityTests
{
    [Fact]
    public async Task Verification_token_can_only_be_used_once()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        await AddUserAsync(
            factory,
            verificationToken: "one-time-verification",
            verificationExpires: DateTime.UtcNow.AddMinutes(10));

        using var first = await client.GetAsync(
            "/Account/VerifyAccount?token=one-time-verification");
        using var second = await client.GetAsync(
            "/Account/VerifyAccount?token=one-time-verification");

        Assert.Equal("/Account/VerificationSuccess", first.Headers.Location?.OriginalString);
        Assert.Equal("/Account/Error", second.Headers.Location?.OriginalString);
        using var scope = factory.Services.CreateScope();
        var user = Assert.Single(
            scope.ServiceProvider.GetRequiredService<DataContext>().Users);
        Assert.Equal(1, user.status);
        Assert.Null(user.verificationToken);
        Assert.Null(user.VerificationTokenExpires);
    }

    [Fact]
    public async Task Expired_verification_token_is_rejected()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        await AddUserAsync(
            factory,
            verificationToken: "expired-verification",
            verificationExpires: DateTime.UtcNow.AddMinutes(-1));

        using var response = await client.GetAsync(
            "/Account/VerifyAccount?token=expired-verification");

        Assert.Equal("/Account/Error", response.Headers.Location?.OriginalString);
        using var scope = factory.Services.CreateScope();
        var user = Assert.Single(
            scope.ServiceProvider.GetRequiredService<DataContext>().Users);
        Assert.Equal(0, user.status);
    }

    [Fact]
    public async Task Expired_password_reset_token_is_rejected()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient();
        await AddUserAsync(
            factory,
            resetToken: "expired-reset",
            resetExpires: DateTime.UtcNow.AddMinutes(-1));

        using var response = await client.PostAsJsonWithCsrfAsync(
            "/Account/ResetPasswordConfirm",
            new
            {
                newPassword = "NewPassword@123",
                reNewPassword = "NewPassword@123",
                token = "expired-reset"
            });
        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Token expired", body, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task AddUserAsync(
        QuizzyWebApplicationFactory factory,
        string? verificationToken = null,
        DateTime? verificationExpires = null,
        string? resetToken = null,
        DateTime? resetExpires = null)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Users.Add(new User
        {
            ID = 95001,
            email = "token-security@quizzy.test",
            fullname = "Token Security",
            password = PasswordServiceTests.LegacyMd5("OldPassword@123"),
            Phone = "0901234567",
            gender = true,
            RoleID = 2,
            status = verificationToken is null ? 1 : 0,
            verificationToken = verificationToken,
            VerificationTokenExpires = verificationExpires,
            PasswordResetToken = resetToken,
            PasswordResetTokenExpires = resetExpires
        });
        await context.SaveChangesAsync();
    }
}
