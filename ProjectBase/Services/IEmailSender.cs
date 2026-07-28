namespace ProjectBase.Services;

public interface IEmailSender
{
    Task SendVerificationLinkAsync(
        string recipient,
        string token,
        CancellationToken cancellationToken = default);

    Task SendPasswordResetLinkAsync(
        string recipient,
        string token,
        CancellationToken cancellationToken = default);
}
