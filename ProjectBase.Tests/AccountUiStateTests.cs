using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Tests;

public sealed class AccountUiStateTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;
    private readonly string _viewsRoot;

    public AccountUiStateTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
        var contentRoot = factory.Services
            .GetRequiredService<IWebHostEnvironment>()
            .ContentRootPath;
        _viewsRoot = Path.Combine(contentRoot, "Views");
    }

    [Theory]
    [InlineData("/Account/VerificationSuccess", "Verification Successful")]
    [InlineData("/Account/Error", "invalid, expired, or has already been used")]
    public async Task Account_status_pages_render_actionable_content(
        string route,
        string expectedText)
    {
        using var client = _factory.CreateClient();

        using var response = await client.GetAsync(route);
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(expectedText, html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("account-status-action", html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("Shared/Login.cshtml", "Logging in…")]
    [InlineData("Shared/Register.cshtml", "Registering…")]
    [InlineData("User/ResetPasswordRequest.cshtml", "Sending…")]
    [InlineData("User/ResetPasswordConfirm.cshtml", "Changing…")]
    public void Account_forms_define_loading_and_restore_states(
        string relativePath,
        string loadingText)
    {
        var view = File.ReadAllText(
            Path.Combine(_viewsRoot, relativePath.Replace('/', Path.DirectorySeparatorChar)));

        Assert.Contains(loadingText, view, StringComparison.Ordinal);
        Assert.Contains("originalButtonHtml", view, StringComparison.Ordinal);
        Assert.Contains("prop('disabled', true)", view, StringComparison.Ordinal);
        Assert.Contains("prop('disabled', false)", view, StringComparison.Ordinal);
    }

    [Fact]
    public void Reset_confirm_renders_server_errors_as_text_nodes()
    {
        var view = File.ReadAllText(
            Path.Combine(_viewsRoot, "User", "ResetPasswordConfirm.cshtml"));

        Assert.Contains("document.createTextNode(error)", view, StringComparison.Ordinal);
        Assert.DoesNotContain("append('<br>' + error)", view, StringComparison.Ordinal);
        Assert.DoesNotContain("Html.Raw(Json.Serialize", view, StringComparison.Ordinal);
    }
}
