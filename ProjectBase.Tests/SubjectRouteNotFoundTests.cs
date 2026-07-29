using System.Net;

namespace ProjectBase.Tests;

public sealed class SubjectRouteNotFoundTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SubjectRouteNotFoundTests(QuizzyWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Details_returns_not_found_for_missing_subject()
    {
        using var response = await _client.GetAsync("/Subjects/Details/999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Subject_popup_returns_not_found_for_missing_subject()
    {
        using var response = await _client.PostAsync(
            "/Subjects/GetSubjectData?subjectId=999&userId=999",
            content: null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
