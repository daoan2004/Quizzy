using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class PracticeCreationIntegrationTests
{
    [Theory]
    [InlineData(1, 9, 1, 0)]
    [InlineData(2, 3, 7, 0)]
    [InlineData(3, 0, 6, 4)]
    public async Task Creates_practice_with_expected_difficulty_distribution(
        int difficulty,
        int expectedLevel1,
        int expectedLevel2,
        int expectedLevel3)
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await SeedRegisteredSubjectAndQuestionsAsync(session, questionsPerLevel: 10);

        using var response = await CreatePracticeAsync(
            session,
            title: $"Difficulty {difficulty}",
            difficulty,
            questionCount: 10);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var practice = Assert.Single(context.Practice);
        var selectedLevels = await context.QuizHandle
            .Join(
                context.QuizBank,
                handle => handle.QuizID,
                question => question.ID,
                (_, question) => question.LevelID)
            .ToListAsync();
        Assert.Equal(10, selectedLevels.Count);
        Assert.Equal(expectedLevel1, selectedLevels.Count(level => level == 1));
        Assert.Equal(expectedLevel2, selectedLevels.Count(level => level == 2));
        Assert.Equal(expectedLevel3, selectedLevels.Count(level => level == 3));
        Assert.Equal(practice.ID, Assert.Single(context.QuizHandle.Select(h => h.PracticeID).Distinct()));
    }

    [Fact]
    public async Task Insufficient_question_bank_leaves_no_partial_data()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await SeedRegisteredSubjectAndQuestionsAsync(session, questionsPerLevel: 2);

        using var response = await CreatePracticeAsync(
            session,
            title: "Not enough questions",
            difficulty: 1,
            questionCount: 10);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains(
            "Not enough",
            await response.Content.ReadAsStringAsync(),
            StringComparison.OrdinalIgnoreCase);
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        Assert.Empty(context.Practice);
        Assert.Empty(context.QuizHandle);
    }

    [Fact]
    public async Task Unregistered_subject_is_forbidden()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await SeedRegisteredSubjectAndQuestionsAsync(
            session,
            questionsPerLevel: 10,
            includeRegistration: false);

        using var response = await CreatePracticeAsync(
            session,
            title: "No registration",
            difficulty: 1,
            questionCount: 10);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        using var scope = session.Factory.Services.CreateScope();
        Assert.Empty(scope.ServiceProvider.GetRequiredService<DataContext>().Practice);
    }

    [Fact]
    public async Task Repeated_create_request_does_not_create_duplicate_practice()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await SeedRegisteredSubjectAndQuestionsAsync(session, questionsPerLevel: 10);

        using var first = await CreatePracticeAsync(
            session, "Double submit", difficulty: 2, questionCount: 10);
        using var second = await CreatePracticeAsync(
            session, "Double submit", difficulty: 2, questionCount: 10);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        Assert.Single(context.Practice);
        Assert.Equal(10, context.QuizHandle.Count());
    }

    [Theory]
    [InlineData("0", "00:30:00", "1", "Question count")]
    [InlineData("-1", "00:30:00", "1", "Question count")]
    [InlineData("10", "invalid", "1", "Duration")]
    [InlineData("10", "00:00:00", "1", "Duration")]
    [InlineData("10", "00:30:00", "4", "Difficulty")]
    public async Task Invalid_practice_fields_are_rejected_without_writes(
        string questionCount,
        string duration,
        string difficulty,
        string expectedMessage)
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["SubjectID"] = "99001",
            ["title"] = "Invalid Practice",
            ["number_quest"] = questionCount,
            ["Quest_group"] = "0",
            ["duration"] = duration,
            ["levelID"] = difficulty
        });

        using var response = await session.PostWithAntiForgeryAsync(
            "/api/PracticeApi/AddPractice",
            content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
            expectedMessage,
            await response.Content.ReadAsStringAsync(),
            StringComparison.OrdinalIgnoreCase);
        using var scope = session.Factory.Services.CreateScope();
        Assert.Empty(scope.ServiceProvider.GetRequiredService<DataContext>().Practice);
    }

    private static async Task<HttpResponseMessage> CreatePracticeAsync(
        AuthenticatedTestSession session,
        string title,
        int difficulty,
        int questionCount)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["SubjectID"] = "99001",
            ["title"] = title,
            ["number_quest"] = questionCount.ToString(),
            ["Quest_group"] = "0",
            ["duration"] = "00:30:00",
            ["levelID"] = difficulty.ToString()
        });
        return await session.PostWithAntiForgeryAsync(
            "/api/PracticeApi/AddPractice",
            content);
    }

    private static async Task SeedRegisteredSubjectAndQuestionsAsync(
        AuthenticatedTestSession session,
        int questionsPerLevel,
        bool includeRegistration = true)
    {
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Subjects.Add(new SubjectsModel
        {
            ID = 99001,
            title = "Practice Creation",
            brief_info = "Test",
            Description = "Test",
            rate = 5
        });
        if (includeRegistration)
        {
            context.Recipe.Add(new RecipeModel
            {
                ID = 99001,
                PricePackage_ID = 1,
                UserID = session.UserId,
                SubjectID = 99001,
                PricePackage_Type = 1,
                BuyAt = DateTime.UtcNow,
                EndAt = DateTime.UtcNow.AddMonths(3),
                Status = RegistrationStatuses.Registered
            });
        }
        for (var level = 1; level <= 3; level++)
        {
            for (var index = 1; index <= questionsPerLevel; index++)
            {
                context.QuizBank.Add(new QuizBankModel
                {
                    ID = level * 1000 + index,
                    SubjectID = 99001,
                    TopicID = 1,
                    LevelID = level,
                    Status = true,
                    GroupID = "A",
                    Title = $"Level {level} Question {index}",
                    QA = "A",
                    QB = "B",
                    QC = "C",
                    QD = "D",
                    QE = "E",
                    QF = "F",
                    Qcorrect = "A"
                });
            }
        }
        await context.SaveChangesAsync();
    }
}
