using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Tests;

public sealed class DoubleSubmitProtectionTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly string _contentRoot;

    public DoubleSubmitProtectionTests(QuizzyWebApplicationFactory factory)
    {
        _contentRoot = factory.Services
            .GetRequiredService<IWebHostEnvironment>()
            .ContentRootPath;
    }

    [Fact]
    public void Shared_form_handler_blocks_repeated_submit_and_disables_button()
    {
        var script = File.ReadAllText(
            Path.Combine(_contentRoot, "wwwroot", "js", "site.js"));

        Assert.Contains("form.data(\"quizlySubmitting\")", script, StringComparison.Ordinal);
        Assert.Contains("event.preventDefault()", script, StringComparison.Ordinal);
        Assert.Contains(".prop(\"disabled\", true)", script, StringComparison.Ordinal);
        Assert.Contains("ajaxComplete", script, StringComparison.Ordinal);
        Assert.Contains("removeData(\"quizlySubmitting\")", script, StringComparison.Ordinal);
    }

    [Fact]
    public void Quiz_answer_recalculates_correct_count_instead_of_incrementing()
    {
        var controller = File.ReadAllText(
            Path.Combine(_contentRoot, "Controllers", "QuizApiController.cs"));

        Assert.Contains("SELECT COUNT(*)", controller, StringComparison.Ordinal);
        Assert.Contains("AND UserID = @UserID", controller, StringComparison.Ordinal);
        Assert.DoesNotContain(
            "number_correct = number_correct + 1",
            controller,
            StringComparison.Ordinal);
    }
}
