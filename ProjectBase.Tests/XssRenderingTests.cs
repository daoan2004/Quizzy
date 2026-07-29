using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Tests;

public sealed class XssRenderingTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly string _contentRoot;

    public XssRenderingTests(QuizzyWebApplicationFactory factory)
    {
        _contentRoot = factory.Services
            .GetRequiredService<IWebHostEnvironment>()
            .ContentRootPath;
    }

    [Fact]
    public void Quiz_review_does_not_render_question_title_as_raw_html()
    {
        var view = Read("Views", "QuizReview", "Detail.cshtml");

        Assert.DoesNotContain(
            "Html.Raw(quiz_review.QuizBank.Title)",
            view,
            StringComparison.Ordinal);
        Assert.Contains(
            "@quiz_review.QuizBank.Title",
            view,
            StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("Views/Dashboard/Index.cshtml", "${reg.subjectTitle}")]
    [InlineData("Views/Dashboard/Index.cshtml", "${reg.status}")]
    [InlineData("Views/Practice/Index.cshtml", "' + item.subject.title + '")]
    [InlineData("Views/Practice/Index.cshtml", "' + item.title + '")]
    [InlineData("Views/Quiz/Handle.cshtml", ".html(response.quizBank.title)")]
    [InlineData("Views/Shared/Register.cshtml", "'<li>' + error + '</li>'")]
    public void User_controlled_values_are_not_concatenated_into_html(
        string relativePath,
        string unsafePattern)
    {
        var source = File.ReadAllText(
            Path.Combine(_contentRoot, relativePath.Replace('/', Path.DirectorySeparatorChar)));

        Assert.DoesNotContain(unsafePattern, source, StringComparison.Ordinal);
    }

    [Fact]
    public void Shared_ui_exposes_text_based_html_escaping()
    {
        var script = Read("wwwroot", "js", "site.js");

        Assert.Contains("element.textContent =", script, StringComparison.Ordinal);
        Assert.Contains("escapeHtml: escapeHtml", script, StringComparison.Ordinal);
    }

    private string Read(params string[] parts) =>
        File.ReadAllText(Path.Combine([_contentRoot, .. parts]));
}
