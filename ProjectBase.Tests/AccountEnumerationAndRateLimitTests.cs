using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;

namespace ProjectBase.Tests;

public sealed class AccountEnumerationAndRateLimitTests
{
    [Fact]
    public async Task Password_reset_response_does_not_reveal_whether_email_exists()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient();
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Users.Add(new User
            {
                ID = 96001,
                email = "known@quizzy.test",
                fullname = "Known User",
                password = PasswordServiceTests.LegacyMd5("Password@123"),
                Phone = "0901234567",
                gender = true,
                RoleID = 2,
                status = 1
            });
            await context.SaveChangesAsync();
        }

        using var knownResponse = await client.PostAsJsonWithCsrfAsync(
            "/Account/ResetPasswordRequest",
            new { email = "known@quizzy.test" });
        using var unknownResponse = await client.PostAsJsonWithCsrfAsync(
            "/Account/ResetPasswordRequest",
            new { email = "unknown@quizzy.test" });
        var known = await ReadResetResponseAsync(knownResponse);
        var unknown = await ReadResetResponseAsync(unknownResponse);

        Assert.True(known.Success);
        Assert.True(unknown.Success);
        Assert.Equal(known.Message, unknown.Message);
        Assert.Contains("If an account exists", known.Message, StringComparison.Ordinal);
        var fakeSender = factory.Services.GetRequiredService<FakeEmailSender>();
        Assert.Single(fakeSender.Messages);
    }

    [Fact]
    public async Task Password_reset_is_rate_limited_after_five_requests()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient();
        var statuses = new List<HttpStatusCode>();

        for (var index = 0; index < 6; index++)
        {
            using var response = await client.PostAsJsonWithCsrfAsync(
                "/Account/ResetPasswordRequest",
                new { email = $"missing-{index}@quizzy.test" });
            statuses.Add(response.StatusCode);
        }

        Assert.All(statuses.Take(5), status => Assert.Equal(HttpStatusCode.OK, status));
        Assert.Equal(HttpStatusCode.TooManyRequests, statuses[5]);
    }

    [Fact]
    public async Task Registration_is_rate_limited_after_five_requests()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient();
        var statuses = new List<HttpStatusCode>();

        for (var index = 0; index < 6; index++)
        {
            using var response = await client.PostAsJsonWithCsrfAsync(
                "/Account/Register",
                new
                {
                    fullname = $"Rate Limit {index}",
                    password = "Customer@123",
                    confirmPassword = "Customer@123",
                    email = $"rate-limit-{index}@quizzy.test",
                    phone = $"09012345{index:D2}",
                    gender = true
                });
            statuses.Add(response.StatusCode);
        }

        Assert.All(statuses.Take(5), status => Assert.Equal(HttpStatusCode.OK, status));
        Assert.Equal(HttpStatusCode.TooManyRequests, statuses[5]);
    }

    private static async Task<(bool Success, string Message)> ReadResetResponseAsync(
        HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return (
            json.RootElement.GetProperty("success").GetBoolean(),
            json.RootElement.GetProperty("message").GetString()!);
    }
}
