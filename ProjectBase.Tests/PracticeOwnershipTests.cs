using System.Net;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class PracticeOwnershipTests
{
    [Theory]
    [InlineData("/Practice/Details/91001")]
    [InlineData("/Quiz/HandleAsync?UserID=91002&PracticeID=91001&isPractice=true")]
    [InlineData("/QuizReview/Detail/91001")]
    [InlineData("/api/QuizApi/getQuestionsList?UserID=91002&PracticeID=91001")]
    public async Task User_cannot_access_another_users_practice(string route)
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await AddPracticeOwnedByAnotherUserAsync(session.Factory);

        using var response = await session.Client.GetAsync(route);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Practice_list_ignores_user_id_supplied_by_client()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await AddPracticeOwnedByAnotherUserAsync(session.Factory);

        using var response = await session.Client.GetAsync(
            "/api/PracticeApi/GetPracticePagination/91002");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("Other user's practice", body, StringComparison.Ordinal);
    }

    private static async Task AddPracticeOwnedByAnotherUserAsync(
        QuizzyWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Practice.Add(new PracticeModel
        {
            ID = 91001,
            UserID = 91002,
            SubjectID = 1,
            title = "Other user's practice",
            taken_date = DateTime.UtcNow,
            duration = new TimeOnly(0, 10),
            number_quest = 10,
            number_correct = 0,
            levelID = 1,
            topicID = 1,
            time_taken = new TimeOnly(0, 0),
            Quest_group = "0",
            Status = false
        });
        await context.SaveChangesAsync();
    }
}
