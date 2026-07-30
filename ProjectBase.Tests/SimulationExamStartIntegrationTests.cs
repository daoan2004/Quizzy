using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class SimulationExamStartIntegrationTests
{
    [Fact]
    public async Task Active_registration_creates_exam_attempt_and_can_resume_it()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await SeedExamScenarioAsync(session, RegistrationStatuses.Registered);

        using var first = await session.PostWithAntiForgeryAsync(
            "/api/SimulationExamApi/Start/98001",
            content: null);
        var firstPracticeId = await ReadPracticeIdAsync(first);
        using var second = await session.PostWithAntiForgeryAsync(
            "/api/SimulationExamApi/Start/98001",
            content: null);
        var secondPracticeId = await ReadPracticeIdAsync(second);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(firstPracticeId, secondPracticeId);

        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var practice = await context.Practice.SingleAsync();
        Assert.Equal(98001, practice.SimulationExamID);
        Assert.Equal(4, practice.number_quest);
        Assert.Equal(4, await context.QuizHandle.CountAsync());
        Assert.All(
            await context.QuizHandle.ToListAsync(),
            handle => Assert.Equal(session.UserId, handle.UserID));

        using var page = await session.Client.GetAsync(
            $"/Quiz/Handle?PracticeID={practice.ID}");
        Assert.Equal(HttpStatusCode.OK, page.StatusCode);
        Assert.Contains("Exam", await page.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Submitted_registration_cannot_list_or_start_exam()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await SeedExamScenarioAsync(session, RegistrationStatuses.Submitted);

        using var list = await session.Client.GetAsync(
            $"/api/SimulationExamApi/GetExamPagination/{session.UserId}");
        using var start = await session.PostWithAntiForgeryAsync(
            "/api/SimulationExamApi/Start/98001",
            content: null);

        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        using var payload = JsonDocument.Parse(
            await list.Content.ReadAsStringAsync());
        Assert.Equal(0, payload.RootElement.GetProperty("totalItems").GetInt32());
        Assert.Equal(HttpStatusCode.Forbidden, start.StatusCode);

        using var scope = session.Factory.Services.CreateScope();
        Assert.Empty(scope.ServiceProvider.GetRequiredService<DataContext>().Practice);
    }

    [Fact]
    public async Task Insufficient_questions_returns_conflict_without_partial_attempt()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await SeedExamScenarioAsync(
            session,
            RegistrationStatuses.Registered,
            questionsPerLevel: 1);

        using var response = await session.PostWithAntiForgeryAsync(
            "/api/SimulationExamApi/Start/98001",
            content: null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        Assert.Empty(context.Practice);
        Assert.Empty(context.QuizHandle);
    }

    [Fact]
    public async Task Level_filter_returns_only_matching_exam()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await SeedExamScenarioAsync(session, RegistrationStatuses.Registered);
        using (var scope = session.Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.SimulationExam.Add(new SimulationExam
            {
                ID = 98002,
                SubjectID = 98001,
                LevelID = 2,
                ExamName = "Medium Exam",
                Number_Question = 4,
                Duration = 20,
                Passrate = 60
            });
            await context.SaveChangesAsync();
        }

        using var response = await session.Client.GetAsync(
            $"/api/SimulationExamApi/GetExamPagination/{session.UserId}?levelId=2");
        using var payload = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        var exam = Assert.Single(payload.RootElement.GetProperty("exams").EnumerateArray());

        Assert.Equal(98002, exam.GetProperty("id").GetInt64());
    }

    private static async Task<long> ReadPracticeIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        using var payload = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());
        return payload.RootElement.GetProperty("practiceId").GetInt64();
    }

    private static async Task SeedExamScenarioAsync(
        AuthenticatedTestSession session,
        string registrationStatus,
        int questionsPerLevel = 3)
    {
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Subjects.Add(new SubjectsModel
        {
            ID = 98001,
            title = "Simulation Subject",
            brief_info = "Test",
            Description = "Test",
            rate = 5
        });
        context.Recipe.Add(new RecipeModel
        {
            ID = 98001,
            PricePackage_ID = 1,
            UserID = session.UserId,
            SubjectID = 98001,
            PricePackage_Type = 1,
            BuyAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddDays(1),
            Status = registrationStatus
        });
        context.SimulationExam.Add(new SimulationExam
        {
            ID = 98001,
            SubjectID = 98001,
            LevelID = 1,
            ExamName = "Entry Exam",
            Number_Question = 4,
            Duration = 20,
            Passrate = 60
        });
        for (var level = 1; level <= 2; level++)
        {
            for (var index = 1; index <= questionsPerLevel; index++)
            {
                context.QuizBank.Add(new QuizBankModel
                {
                    ID = level * 10000 + index,
                    SubjectID = 98001,
                    TopicID = 1,
                    LevelID = level,
                    Status = true,
                    GroupID = "A",
                    Title = $"Question {level}-{index}",
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
