using System.Net;

namespace ProjectBase.Tests;

public sealed class PublicRouteSmokeTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PublicRouteSmokeTests(QuizzyWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new()
        {
            AllowAutoRedirect = false
        });
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Subjects")]
    [InlineData("/Blogs")]
    [InlineData("/Account/ResetPasswordRequest")]
    public async Task Public_route_returns_success_without_using_demo_database(string route)
    {
        using var response = await _client.GetAsync(route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }
}
