using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Tests;

public sealed class PracticeCreationUiTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly string _view;
    private readonly string _script;

    public PracticeCreationUiTests(QuizzyWebApplicationFactory factory)
    {
        var contentRoot = factory.Services
            .GetRequiredService<IWebHostEnvironment>()
            .ContentRootPath;
        _view = File.ReadAllText(
            Path.Combine(contentRoot, "Views", "Practice", "NewPractice.cshtml"));
        _script = File.ReadAllText(
            Path.Combine(contentRoot, "wwwroot", "js", "NewPractice.js"));
    }

    [Fact]
    public void Successful_creation_navigates_to_returned_practice_id()
    {
        Assert.Contains("src=\"~/js/NewPractice.js\"", _view, StringComparison.Ordinal);
        Assert.Contains("practiceID = response", _script, StringComparison.Ordinal);
        Assert.Contains(
            "'&PracticeID=' + practiceID",
            _script,
            StringComparison.Ordinal);
        Assert.Contains("'&IsPractice=' + isPractice", _script, StringComparison.Ordinal);
    }

    [Fact]
    public void Creation_button_shows_loading_and_server_error()
    {
        Assert.Contains("Creating…", _script, StringComparison.Ordinal);
        Assert.Contains("xhr.responseJSON?.message", _script, StringComparison.Ordinal);
        Assert.Contains("submitButton.prop('disabled', true)", _script, StringComparison.Ordinal);
        Assert.Contains("submitButton.prop('disabled', false)", _script, StringComparison.Ordinal);
    }
}
