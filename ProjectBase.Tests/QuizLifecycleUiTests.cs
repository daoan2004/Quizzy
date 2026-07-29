using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Tests;

public sealed class QuizLifecycleUiTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly string _view;

    public QuizLifecycleUiTests(QuizzyWebApplicationFactory factory)
    {
        var contentRoot = factory.Services
            .GetRequiredService<IWebHostEnvironment>()
            .ContentRootPath;
        _view = File.ReadAllText(
            Path.Combine(contentRoot, "Views", "Quiz", "Handle.cshtml"));
    }

    [Fact]
    public void Previous_and_next_use_question_array_instead_of_incrementing_ids()
    {
        Assert.Contains(
            "response[currentQuestionIndex - 1].id",
            _view,
            StringComparison.Ordinal);
        Assert.DoesNotContain("currentQuestionID++", _view, StringComparison.Ordinal);
        Assert.DoesNotContain("currentQuestionID--", _view, StringComparison.Ordinal);
    }

    [Fact]
    public void Mark_controls_call_server_toggle_endpoint()
    {
        Assert.Contains("/api/QuizApi/toggleMark", _view, StringComparison.Ordinal);
        Assert.Contains("isMarked: shouldMark", _view, StringComparison.Ordinal);
    }

    [Fact]
    public void Timer_uses_server_deadline_and_not_local_storage()
    {
        Assert.Contains("AttemptEndsAtUtc", _view, StringComparison.Ordinal);
        Assert.Contains("deadline.getTime() - Date.now()", _view, StringComparison.Ordinal);
        Assert.DoesNotContain("localStorage", _view, StringComparison.Ordinal);
    }

    [Fact]
    public void Multiple_choice_uses_selection_limit_without_correct_answer()
    {
        Assert.Contains("quizBank.selectionLimit", _view, StringComparison.Ordinal);
        Assert.DoesNotContain("quizBank.qcorrect", _view, StringComparison.OrdinalIgnoreCase);
    }
}
