using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class QuizLifecycleIntegrationTests
{
    [Fact]
    public async Task Question_responses_do_not_expose_correct_answer()
    {
        await using var session = await CreateAttemptAsync();

        using var listResponse = await session.Client.GetAsync(
            $"/api/QuizApi/getQuestionsList?UserID=999&PracticeID=99501");
        using var detailResponse = await session.Client.GetAsync(
            "/api/QuizApi/loadQuestion/99501");
        var list = await listResponse.Content.ReadAsStringAsync();
        var detail = await detailResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        Assert.DoesNotContain("qCorrect", list, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("qCorrect", detail, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("selectionLimit", detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Selecting_and_changing_answer_recalculates_score_and_survives_refresh()
    {
        await using var session = await CreateAttemptAsync();

        using var correct = await SubmitAnswerAsync(session, "A");
        Assert.Equal(HttpStatusCode.OK, correct.StatusCode);
        Assert.True(ReadHandle(session).isCorrect);
        Assert.Equal(1, ReadPractice(session).number_correct);

        using var changed = await SubmitAnswerAsync(session, "B");
        Assert.Equal(HttpStatusCode.OK, changed.StatusCode);
        Assert.False(ReadHandle(session).isCorrect);
        Assert.Equal(0, ReadPractice(session).number_correct);

        using var refresh = await session.Client.GetAsync(
            "/api/QuizApi/loadQuestion/99501");
        using var json = JsonDocument.Parse(await refresh.Content.ReadAsStringAsync());
        Assert.Equal("B", json.RootElement.GetProperty("qAnswer").GetString());
        Assert.True(json.RootElement.GetProperty("status").GetBoolean());
    }

    [Fact]
    public async Task Mark_and_unmark_are_persisted()
    {
        await using var session = await CreateAttemptAsync();

        using var mark = await ToggleMarkAsync(session, true);
        Assert.Equal(HttpStatusCode.OK, mark.StatusCode);
        Assert.True(ReadHandle(session).isMark);

        using var unmark = await ToggleMarkAsync(session, false);
        Assert.Equal(HttpStatusCode.OK, unmark.StatusCode);
        Assert.False(ReadHandle(session).isMark);
    }

    [Fact]
    public async Task Finished_attempt_cannot_be_submitted_or_modified()
    {
        await using var session = await CreateAttemptAsync();

        using var finish = await session.PostWithAntiForgeryAsync(
            "/api/QuizApi/finishAttempt?UserID=999&PracticeID=99501",
            null);
        using var finishAgain = await session.PostWithAntiForgeryAsync(
            "/api/QuizApi/finishAttempt?UserID=999&PracticeID=99501",
            null);
        using var answer = await SubmitAnswerAsync(session, "A");
        using var mark = await ToggleMarkAsync(session, true);

        Assert.Equal(HttpStatusCode.OK, finish.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, finishAgain.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, answer.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, mark.StatusCode);
        Assert.False(ReadHandle(session).status);
    }

    [Fact]
    public async Task Expired_attempt_is_closed_by_server_and_rejects_answer()
    {
        await using var session = await CreateAttemptAsync(expired: true);

        using var answer = await SubmitAnswerAsync(session, "A");

        Assert.Equal(HttpStatusCode.Gone, answer.StatusCode);
        Assert.True(ReadPractice(session).Status);
        Assert.False(ReadHandle(session).status);
    }

    private static async Task<AuthenticatedTestSession> CreateAttemptAsync(
        bool expired = false)
    {
        var session = await AuthenticatedTestSession.CreateAsync();
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Practice.Add(new PracticeModel
        {
            ID = 99501,
            UserID = session.UserId,
            SubjectID = 1,
            title = "Lifecycle Attempt",
            taken_date = expired ? DateTime.UtcNow.AddHours(-1) : DateTime.UtcNow,
            duration = new TimeOnly(0, 30),
            number_quest = 1,
            number_correct = 0,
            levelID = 1,
            topicID = 1,
            time_taken = TimeOnly.MinValue,
            Quest_group = "0",
            Status = false
        });
        context.QuizBank.Add(new QuizBankModel
        {
            ID = 99501,
            SubjectID = 1,
            TopicID = 1,
            LevelID = 1,
            Status = true,
            GroupID = "1",
            Title = "Lifecycle Question",
            QA = "A",
            QB = "B",
            QC = "C",
            QD = "D",
            QE = string.Empty,
            QF = string.Empty,
            Qcorrect = "A"
        });
        context.QuizHandle.Add(new QuizHandleModel
        {
            ID = 99501,
            UserID = session.UserId,
            PracticeID = 99501,
            QuizID = 99501,
            QAnswer = string.Empty,
            isMark = false,
            status = false,
            isCorrect = false
        });
        await context.SaveChangesAsync();
        return session;
    }

    private static Task<HttpResponseMessage> SubmitAnswerAsync(
        AuthenticatedTestSession session,
        string answer) =>
        session.PostWithAntiForgeryAsync(
            "/api/QuizApi/submitAnswer",
            Form(("questionId", "99501"), ("answer", answer), ("PracticeID", "99501")));

    private static Task<HttpResponseMessage> ToggleMarkAsync(
        AuthenticatedTestSession session,
        bool isMarked) =>
        session.PostWithAntiForgeryAsync(
            "/api/QuizApi/toggleMark",
            Form(
                ("questionId", "99501"),
                ("PracticeID", "99501"),
                ("isMarked", isMarked.ToString())));

    private static FormUrlEncodedContent Form(
        params (string Key, string Value)[] values) =>
        new(values.ToDictionary(value => value.Key, value => value.Value));

    private static PracticeModel ReadPractice(AuthenticatedTestSession session)
    {
        using var scope = session.Factory.Services.CreateScope();
        return scope.ServiceProvider
            .GetRequiredService<DataContext>()
            .Practice.Single(practice => practice.ID == 99501);
    }

    private static QuizHandleModel ReadHandle(AuthenticatedTestSession session)
    {
        using var scope = session.Factory.Services.CreateScope();
        return scope.ServiceProvider
            .GetRequiredService<DataContext>()
            .QuizHandle.Single(handle => handle.ID == 99501);
    }
}
