using System.Net;

namespace ProjectBase.Tests;

public sealed class PracticeApiValidationTests
{
    [Fact]
    public async Task Add_practice_rejects_missing_form_values_before_database_write()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["UserID"] = "invalid",
            ["SubjectID"] = "0",
            ["number_quest"] = "-1",
            ["levelID"] = "9"
        });

        using var response = await session.PostWithAntiForgeryAsync("/api/PracticeApi/AddPractice", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
            "Invalid practice data",
            await response.Content.ReadAsStringAsync(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Add_practice_requires_form_content_type()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        using var response = await session.PostWithAntiForgeryAsync(
            "/api/PracticeApi/AddPractice",
            new StringContent("{}"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
