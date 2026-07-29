using System.Net;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public class QuizReviewIntegrationTests
{
    [Fact]
    public async Task Index_RouteIsRegistered()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();

        using var response = await session.Client.GetAsync("/QuizReview");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/Practice", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Detail_CompletedAttempt_RendersDatabaseSummaryAndAnswerStates()
    {
        await using var session = await CreateReviewAsync(completed: true);

        using var response = await session.Client.GetAsync("/QuizReview/Detail?id=99601");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Number of Questions:</strong> 3", html);
        Assert.Contains("Number of Correct Answers:</strong> 1", html);
        Assert.Contains("Number of Wrong Answers:</strong> 1", html);
        Assert.Contains("Number of Unanswered Questions:</strong> 1", html);
        Assert.Contains("Your Answer:</strong> A", html);
        Assert.Contains("Your Answer:</strong> B", html);
        Assert.Contains("Your Answer:</strong> Not answered", html);
        Assert.Contains("Correct Answer:</strong> A", html);
        Assert.Contains("data-result=\"correct\"", html);
        Assert.Contains("data-result=\"incorrect\"", html);
        Assert.Contains("data-result=\"unanswered\"", html);
    }

    [Fact]
    public async Task Detail_UnfinishedAttempt_ReturnsConflictWithoutAnswers()
    {
        await using var session = await CreateReviewAsync(completed: false);

        using var response = await session.Client.GetAsync("/QuizReview/Detail?id=99601");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.DoesNotContain("Correct Answer:", body);
    }

    private static async Task<AuthenticatedTestSession> CreateReviewAsync(bool completed)
    {
        var session = await AuthenticatedTestSession.CreateAsync();
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var subject = context.Subjects.FirstOrDefault() ?? new SubjectsModel
        {
            ID = 99650,
            title = "Review Subject",
            brief_info = "Review test subject",
            Description = "Review test subject",
            rate = 5
        };
        var level = context.PracticeLevel.FirstOrDefault() ?? new PracticeLevel
        {
            ID = 99650,
            title = "Review Level",
            Description = "Review test level"
        };
        var topic = context.SubjectTopic.FirstOrDefault() ?? new SubjectTopicModel
        {
            id = 99650,
            subjectId = subject.ID,
            title = "Review Topic"
        };
        if (context.Entry(subject).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
        {
            context.Subjects.Add(subject);
        }
        if (context.Entry(level).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
        {
            context.PracticeLevel.Add(level);
        }
        if (context.Entry(topic).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
        {
            context.SubjectTopic.Add(topic);
        }

        context.Practice.Add(new PracticeModel
        {
            ID = 99601,
            UserID = session.UserId,
            SubjectID = subject.ID,
            title = "Review Attempt",
            taken_date = DateTime.UtcNow,
            duration = new TimeOnly(0, 30),
            number_quest = 99,
            number_correct = 99,
            levelID = level.ID,
            topicID = topic.id,
            time_taken = new TimeOnly(0, 10),
            Quest_group = "0",
            Status = completed
        });

        for (var index = 0; index < 3; index++)
        {
            var id = 99601 + index;
            context.QuizBank.Add(new QuizBankModel
            {
                ID = id,
                SubjectID = subject.ID,
                TopicID = topic.id,
                LevelID = level.ID,
                Status = true,
                GroupID = "2",
                Title = $"Review Question {index + 1}",
                QA = "Answer A",
                QB = "Answer B",
                QC = string.Empty,
                QD = string.Empty,
                QE = string.Empty,
                QF = string.Empty,
                Qcorrect = "A"
            });
            context.QuizHandle.Add(new QuizHandleModel
            {
                ID = id,
                UserID = session.UserId,
                PracticeID = 99601,
                QuizID = id,
                QAnswer = index == 0 ? "A" : index == 1 ? "B" : string.Empty,
                status = index < 2,
                isCorrect = index == 0
            });
        }

        await context.SaveChangesAsync();
        return session;
    }
}
