using System.Collections.Concurrent;
using ProjectBase.Services;

namespace ProjectBase.Tests;

public sealed record RecordedEmail(
    string Type,
    string Recipient,
    string Link);

public sealed class FakeEmailSender : IEmailSender
{
    private readonly IAccountLinkBuilder _linkBuilder;
    private readonly ConcurrentQueue<RecordedEmail> _messages = new();

    public FakeEmailSender(IAccountLinkBuilder linkBuilder)
    {
        _linkBuilder = linkBuilder;
    }

    public IReadOnlyCollection<RecordedEmail> Messages => _messages.ToArray();

    public Task SendVerificationLinkAsync(
        string recipient,
        string token,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _messages.Enqueue(new(
            "Verification",
            recipient,
            _linkBuilder.BuildVerificationLink(token)));
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(
        string recipient,
        string token,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _messages.Enqueue(new(
            "PasswordReset",
            recipient,
            _linkBuilder.BuildPasswordResetLink(token)));
        return Task.CompletedTask;
    }

    public void Clear()
    {
        while (_messages.TryDequeue(out _))
        {
        }
    }
}
