using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Tests;

public sealed class QuizLifecycleUiTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly string _view;
    private readonly string _script;

    public QuizLifecycleUiTests(QuizzyWebApplicationFactory factory)
    {
        var contentRoot = factory.Services
            .GetRequiredService<IWebHostEnvironment>()
            .ContentRootPath;
        _view = File.ReadAllText(
            Path.Combine(contentRoot, "Views", "Quiz", "Handle.cshtml"));
        _script = File.ReadAllText(
            Path.Combine(contentRoot, "wwwroot", "js", "QuizHandle.js"));
    }

    [Fact]
    public void Previous_and_next_use_question_array_instead_of_incrementing_ids()
    {
        Assert.Contains(
            "response[currentQuestionIndex - 1].id",
            _script,
            StringComparison.Ordinal);
        Assert.DoesNotContain("currentQuestionID++", _script, StringComparison.Ordinal);
        Assert.DoesNotContain("currentQuestionID--", _script, StringComparison.Ordinal);
    }

    [Fact]
    public void Mark_controls_call_server_toggle_endpoint()
    {
        Assert.Contains("/api/QuizApi/toggleMark", _script, StringComparison.Ordinal);
        Assert.Contains("isMarked: shouldMark", _script, StringComparison.Ordinal);
    }

    [Fact]
    public void Timer_uses_server_deadline_and_not_local_storage()
    {
        Assert.Contains("AttemptEndsAtUtc", _view, StringComparison.Ordinal);
        Assert.Contains("data-user-id=\"@Model.UserID\"", _view, StringComparison.Ordinal);
        Assert.Contains("data-practice-id=\"@Model.ID\"", _view, StringComparison.Ordinal);
        Assert.Contains("src=\"~/js/QuizHandle.js\"", _view, StringComparison.Ordinal);
        Assert.Contains("var practiceId = 0;", _script, StringComparison.Ordinal);
        Assert.Contains("practiceId = Number(quizPage.dataset.practiceId)", _script, StringComparison.Ordinal);
        Assert.DoesNotContain("var practiceId = Number", _script, StringComparison.Ordinal);
        Assert.Contains("deadline.getTime() - Date.now()", _script, StringComparison.Ordinal);
        Assert.DoesNotContain("localStorage", _script, StringComparison.Ordinal);
    }

    [Fact]
    public void Multiple_choice_uses_selection_limit_without_correct_answer()
    {
        Assert.Contains("quizBank.selectionLimit", _script, StringComparison.Ordinal);
        Assert.DoesNotContain("quizBank.qcorrect", _script, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void QuizUsesInlineFeedbackBusyAndDisabledStatesInsteadOfAlerts()
    {
        Assert.Contains("id=\"quizFeedback\"", _view, StringComparison.Ordinal);
        Assert.Contains("showQuizFeedback", _script, StringComparison.Ordinal);
        Assert.Contains("setQuizBusy(true)", _script, StringComparison.Ordinal);
        Assert.Contains("Finishing…", _script, StringComparison.Ordinal);
        Assert.Contains("prop('disabled', true)", _script, StringComparison.Ordinal);
        Assert.DoesNotContain("alert(", _script, StringComparison.Ordinal);
    }

    [Fact]
    public void QuizEscapesQuestionAnswerTextBeforeBuildingOptionMarkup()
    {
        Assert.Contains("escapeQuizText", _script, StringComparison.Ordinal);
        Assert.Contains("escapeHtml(value)", _script, StringComparison.Ordinal);
        Assert.Contains("escapeQuizText(response.quizBank.qa)", _script, StringComparison.Ordinal);
        Assert.Contains("escapeQuizText(selectedAnswer)", _script, StringComparison.Ordinal);
    }
}
