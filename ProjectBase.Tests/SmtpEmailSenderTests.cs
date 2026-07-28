using Microsoft.Extensions.Options;
using ProjectBase.Services;

namespace ProjectBase.Tests;

public sealed class SmtpEmailSenderTests
{
    [Fact]
    public async Task Missing_smtp_settings_return_clear_error_before_network_access()
    {
        var options = Options.Create(new EmailOptions
        {
            BaseUrl = "https://quizzy.test/"
        });
        var sender = new SmtpEmailSender(
            options,
            new AccountLinkBuilder(options));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sender.SendVerificationLinkAsync(
                "recipient@quizzy.test",
                "token"));

        Assert.Contains("SMTP is not configured", exception.Message);
        Assert.Contains("Email:Password", exception.Message);
    }
}
