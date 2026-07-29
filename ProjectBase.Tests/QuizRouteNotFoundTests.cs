using System.Net;

namespace ProjectBase.Tests;

public sealed class QuizRouteNotFoundTests
{
    [Theory]
    [InlineData("/Quiz/HandleAsync?UserID=999&PracticeID=999&isPractice=true")]
    [InlineData("/api/QuizApi/loadQuestion/999")]
    [InlineData("/QuizReview/Detail/999")]
    public async Task Missing_quiz_resources_return_not_found(string route)
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        using var response = await session.Client.GetAsync(route);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Submit_answer_returns_not_found_before_database_update()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["questionId"] = "999",
            ["answer"] = "A",
            ["PracticeID"] = "999"
        });
        using var response = await session.PostWithAntiForgeryAsync("/api/QuizApi/submitAnswer", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
