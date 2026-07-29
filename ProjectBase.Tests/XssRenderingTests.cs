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
    [InlineData("wwwroot/js/Dashboard.js", "${reg.subjectTitle}")]
    [InlineData("wwwroot/js/Dashboard.js", "${reg.status}")]
    [InlineData("wwwroot/js/PracticeList.js", "' + item.subject.title + '")]
    [InlineData("wwwroot/js/PracticeList.js", "' + item.title + '")]
    [InlineData("wwwroot/js/QuizHandle.js", ".html(response.quizBank.title)")]
    [InlineData("wwwroot/js/AccountRegister.js", "'<li>' + error + '</li>'")]
    [InlineData("wwwroot/js/BlogIndex.js", "'<span>' + category.title")]
    [InlineData("wwwroot/js/BlogDetail.js", "user.fullname +")]
    [InlineData("wwwroot/js/SimulationExam.js", "' + exam.examName + '")]
    [InlineData("wwwroot/js/SimulationExam.js", "' + item.subjects.title + '")]
    [InlineData("wwwroot/js/QuizHandle.js", "' + response.quizBank.qa + '")]
    [InlineData("wwwroot/js/QuizHandle.js", "'+selectedAnswer+'")]
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
