using Microsoft.Extensions.Logging;
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
        var logger = new RecordingLogger<SmtpEmailSender>();
        var sender = new SmtpEmailSender(
            options,
            new AccountLinkBuilder(options),
            logger);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sender.SendVerificationLinkAsync(
                "recipient@quizzy.test",
                "token"));

        Assert.Contains("SMTP is not configured", exception.Message);
        Assert.Contains("Email:Password", exception.Message);
        Assert.Contains(logger.Properties, property =>
            property.Key == "MissingSettings" &&
            property.Value?.ToString()?.Contains("Email:Password") == true);
        Assert.DoesNotContain(
            logger.Properties,
            property => property.Value?.ToString()?.Contains("token") == true);
    }

    private sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<KeyValuePair<string, object?>> Properties { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (state is IEnumerable<KeyValuePair<string, object?>> properties)
            {
                Properties.AddRange(properties);
            }
        }
    }
}
