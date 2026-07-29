using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Tests;

public sealed class ValidationFeedbackTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly string _viewsRoot;
    private readonly string _scriptsRoot;

    public ValidationFeedbackTests(QuizzyWebApplicationFactory factory)
    {
        var contentRoot = factory.Services
            .GetRequiredService<IWebHostEnvironment>()
            .ContentRootPath;
        _viewsRoot = Path.Combine(contentRoot, "Views");
        _scriptsRoot = Path.Combine(contentRoot, "wwwroot", "js");
    }

    [Fact]
    public void Register_displays_server_validation_errors_as_text()
    {
        var view = File.ReadAllText(
            Path.Combine(_scriptsRoot, "AccountRegister.js"));

        Assert.Contains("response.errors.forEach", view, StringComparison.Ordinal);
        Assert.Contains("$('<li>').text(error)", view, StringComparison.Ordinal);
        Assert.Contains("registerErrorMessage", view, StringComparison.Ordinal);
    }

    [Fact]
    public void Login_has_a_visible_safe_error_target()
    {
        var view = File.ReadAllText(
            Path.Combine(_scriptsRoot, "AccountLogin.js"));

        Assert.Contains("loginErrorMessage", view, StringComparison.Ordinal);
        Assert.Contains(".text(response.message).show()", view, StringComparison.Ordinal);
        Assert.DoesNotContain(
            ".html(response.message)",
            view,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Password_reset_errors_are_rendered_as_text()
    {
        var view = File.ReadAllText(
            Path.Combine(_viewsRoot, "User", "ResetPasswordRequest.cshtml"));

        Assert.Contains(".text('Error: '", view, StringComparison.Ordinal);
        Assert.DoesNotContain(
            "xhr.responseText + '</div>'",
            view,
            StringComparison.Ordinal);
    }
}
