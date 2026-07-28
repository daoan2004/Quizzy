using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;

namespace ProjectBase.Tests;

public sealed class EmailFlowIntegrationTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public EmailFlowIntegrationTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_uses_fake_sender_and_does_not_require_smtp()
    {
        await _factory.ResetDatabaseAsync();
        var fakeSender = _factory.Services.GetRequiredService<FakeEmailSender>();
        fakeSender.Clear();

        using var response = await _client.PostAsJsonAsync("/Account/Register", new
        {
            fullname = "Test Customer",
            password = "Customer@123",
            confirmPassword = "Customer@123",
            email = "register-test@quizzy.test",
            phone = "0901234567",
            gender = true
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(payload.RootElement.GetProperty("success").GetBoolean());

        var email = Assert.Single(fakeSender.Messages);
        Assert.Equal("Verification", email.Type);
        Assert.Equal("register-test@quizzy.test", email.Recipient);
        Assert.StartsWith(
            "https://quizzy.test/Account/VerifyAccount?token=",
            email.Link);
    }

    [Fact]
    public async Task Reset_password_uses_fake_sender_and_records_reset_link()
    {
        await _factory.ResetDatabaseAsync();
        var fakeSender = _factory.Services.GetRequiredService<FakeEmailSender>();
        fakeSender.Clear();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Users.Add(new User
            {
                ID = 50001,
                email = "reset-test@quizzy.test",
                fullname = "Reset Test",
                password = "not-used-by-this-test",
                Phone = "0901234567",
                gender = true,
                RoleID = 2,
                status = 1
            });
            await context.SaveChangesAsync();
        }

        using var response = await _client.PostAsJsonAsync(
            "/Account/ResetPasswordRequest",
            new { email = "reset-test@quizzy.test" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(payload.RootElement.GetProperty("success").GetBoolean());
        Assert.False(payload.RootElement.TryGetProperty("token", out _));

        var email = Assert.Single(fakeSender.Messages);
        Assert.Equal("PasswordReset", email.Type);
        Assert.Equal("reset-test@quizzy.test", email.Recipient);
        Assert.StartsWith(
            "https://quizzy.test/Account/ResetPasswordConfirm?token=",
            email.Link);
    }
}
