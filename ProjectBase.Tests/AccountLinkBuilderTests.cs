using Microsoft.Extensions.Options;
using ProjectBase.Services;

namespace ProjectBase.Tests;

public sealed class AccountLinkBuilderTests
{
    [Fact]
    public void Builds_verification_and_reset_links_from_configured_base_url()
    {
        var builder = CreateBuilder("https://quizzy.example/app/");

        Assert.Equal(
            "https://quizzy.example/app/Account/VerifyAccount?token=a%2Fb%3Fc",
            builder.BuildVerificationLink("a/b?c"));
        Assert.Equal(
            "https://quizzy.example/app/Account/ResetPasswordConfirm?token=reset-token",
            builder.BuildPasswordResetLink("reset-token"));
    }

    [Fact]
    public void Missing_base_url_returns_clear_configuration_error()
    {
        var builder = CreateBuilder(string.Empty);

        var exception = Assert.Throws<InvalidOperationException>(
            () => builder.BuildVerificationLink("token"));

        Assert.Contains("Email:BaseUrl", exception.Message);
    }

    private static AccountLinkBuilder CreateBuilder(string baseUrl) =>
        new(Options.Create(new EmailOptions { BaseUrl = baseUrl }));
}
