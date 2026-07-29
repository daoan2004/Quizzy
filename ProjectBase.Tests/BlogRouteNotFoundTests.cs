using System.Net;

namespace ProjectBase.Tests;

public sealed class BlogRouteNotFoundTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BlogRouteNotFoundTests(QuizzyWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Blog_detail_returns_not_found_for_missing_blog()
    {
        using var response = await _client.GetAsync("/Blogs/BlogsDetail?blogid=999&userid=999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
