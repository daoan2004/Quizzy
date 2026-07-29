using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Tests;

public sealed class RegistrationUiStateTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly string _contentRoot;

    public RegistrationUiStateTests(QuizzyWebApplicationFactory factory)
    {
        _contentRoot = factory.Services
            .GetRequiredService<IWebHostEnvironment>()
            .ContentRootPath;
    }

    [Fact]
    public void Payment_and_cancel_buttons_show_loading_and_server_errors()
    {
        var view = File.ReadAllText(
            Path.Combine(_contentRoot, "Views", "MyRegistrations", "Index.cshtml"));
        var script = File.ReadAllText(
            Path.Combine(_contentRoot, "wwwroot", "js", "MyRegistrations.js"));

        Assert.Contains("src=\"~/js/MyRegistrations.js\"", view, StringComparison.Ordinal);
        Assert.DoesNotContain("<script type=\"text/javascript\">", view, StringComparison.Ordinal);
        Assert.Contains("Cancelling…", script, StringComparison.Ordinal);
        Assert.Contains("Paying…", script, StringComparison.Ordinal);
        Assert.Contains("xhr.responseJSON?.message", script, StringComparison.Ordinal);
        Assert.Contains("button.prop('disabled', true)", script, StringComparison.Ordinal);
        Assert.Contains("button.prop('disabled', false)", script, StringComparison.Ordinal);
        Assert.Contains(".attr('aria-hidden', 'false')", script, StringComparison.Ordinal);
        Assert.Contains(".attr('aria-hidden', 'true')", script, StringComparison.Ordinal);
        Assert.Contains("trigger('focus')", script, StringComparison.Ordinal);
    }

    [Fact]
    public void Paid_registration_does_not_render_edit_package_action()
    {
        var partial = File.ReadAllText(
            Path.Combine(_contentRoot, "Views", "Shared", "_SubjectPopupPartial.cshtml"));

        Assert.Contains(
            "Model.UserRegistration?.Status == RegistrationStatuses.Registered",
            partial,
            StringComparison.Ordinal);
        Assert.Contains(
            "This subject is active on your account.",
            partial,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Subject_registration_uses_a_relational_transaction()
    {
        var controller = File.ReadAllText(
            Path.Combine(_contentRoot, "Controllers", "SubjectRegisterController.cs"));

        Assert.Contains("BeginTransactionAsync", controller, StringComparison.Ordinal);
        Assert.Contains("CommitAsync", controller, StringComparison.Ordinal);
    }
}
