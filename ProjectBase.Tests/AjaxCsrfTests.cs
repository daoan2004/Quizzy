using System.Net;
using System.Net.Http.Json;

namespace ProjectBase.Tests;

public sealed class AjaxCsrfTests
{
    [Theory]
    [InlineData("/api/MyRegistrationsApi/CancelRegistration/999")]
    [InlineData("/api/MyRegistrationsApi/PayPackage/999")]
    [InlineData("/api/PracticeApi/AddPractice")]
    [InlineData("/api/QuizApi/submitAnswer")]
    [InlineData("/api/QuizApi/finishAttempt?UserID=1&PracticeID=999")]
    public async Task Authenticated_ajax_mutation_without_csrf_header_is_rejected(
        string route)
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["questionId"] = "999",
            ["answer"] = "A",
            ["PracticeID"] = "999"
        });

        using var response = await session.Client.PostAsync(route, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("/Account/Register")]
    [InlineData("/Account/Login")]
    [InlineData("/Account/ResetPasswordRequest")]
    [InlineData("/Account/ResetPasswordConfirm")]
    public async Task Anonymous_account_mutation_without_csrf_header_is_rejected(
        string route)
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(route, new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("/Account/ChangePassword")]
    [InlineData("/Account/UpdateUserProfile")]
    public async Task Authenticated_account_mutation_without_csrf_header_is_rejected(
        string route)
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();

        using var response = await session.Client.PostAsJsonAsync(route, new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
