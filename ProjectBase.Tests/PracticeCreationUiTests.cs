using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Tests;

public sealed class PracticeCreationUiTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly string _view;

    public PracticeCreationUiTests(QuizzyWebApplicationFactory factory)
    {
        var contentRoot = factory.Services
            .GetRequiredService<IWebHostEnvironment>()
            .ContentRootPath;
        _view = File.ReadAllText(
            Path.Combine(contentRoot, "Views", "Practice", "NewPractice.cshtml"));
    }

    [Fact]
    public void Successful_creation_navigates_to_returned_practice_id()
    {
        Assert.Contains("practiceID = response", _view, StringComparison.Ordinal);
        Assert.Contains(
            "'&PracticeID=' + practiceID",
            _view,
            StringComparison.Ordinal);
        Assert.Contains("'&IsPractice=' + isPractice", _view, StringComparison.Ordinal);
    }

    [Fact]
    public void Creation_button_shows_loading_and_server_error()
    {
        Assert.Contains("Creating…", _view, StringComparison.Ordinal);
        Assert.Contains("xhr.responseJSON?.message", _view, StringComparison.Ordinal);
        Assert.Contains("submitButton.prop('disabled', true)", _view, StringComparison.Ordinal);
        Assert.Contains("submitButton.prop('disabled', false)", _view, StringComparison.Ordinal);
    }
}
