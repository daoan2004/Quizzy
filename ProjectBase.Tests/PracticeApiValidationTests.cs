using System.Net;

namespace ProjectBase.Tests;

public sealed class PracticeApiValidationTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PracticeApiValidationTests(QuizzyWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Add_practice_rejects_missing_form_values_before_database_write()
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["UserID"] = "invalid",
            ["SubjectID"] = "0",
            ["number_quest"] = "-1",
            ["levelID"] = "9"
        });

        using var response = await _client.PostAsync("/api/PracticeApi/AddPractice", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
            "Invalid practice data",
            await response.Content.ReadAsStringAsync(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Add_practice_requires_form_content_type()
    {
        using var response = await _client.PostAsync(
            "/api/PracticeApi/AddPractice",
            new StringContent("{}"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
