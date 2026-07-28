using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;

namespace ProjectBase.Services;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly IAccountLinkBuilder _linkBuilder;

    public SmtpEmailSender(
        IOptions<EmailOptions> options,
        IAccountLinkBuilder linkBuilder)
    {
        _options = options.Value;
        _linkBuilder = linkBuilder;
    }

    public Task SendVerificationLinkAsync(
        string recipient,
        string token,
        CancellationToken cancellationToken = default)
    {
        var link = _linkBuilder.BuildVerificationLink(token);
        return SendAsync(
            recipient,
            "[Quizly] Verify your email address",
            $"Select <a href=\"{HtmlEncoder.Default.Encode(link)}\">this link</a> to verify your Quizly account.",
            cancellationToken);
    }

    public Task SendPasswordResetLinkAsync(
        string recipient,
        string token,
        CancellationToken cancellationToken = default)
    {
        var link = _linkBuilder.BuildPasswordResetLink(token);
        return SendAsync(
            recipient,
            "[Quizly] Reset your password",
            $"Select <a href=\"{HtmlEncoder.Default.Encode(link)}\">this link</a> to reset your Quizly password.",
            cancellationToken);
    }

    private async Task SendAsync(
        string recipient,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        ValidateConfiguration();
        cancellationToken.ThrowIfCancellationRequested();

        var fromAddress = new MailAddress(_options.FromAddress, _options.FromName);
        var toAddress = new MailAddress(recipient);
        var username = string.IsNullOrWhiteSpace(_options.Username)
            ? _options.FromAddress
            : _options.Username;

        using var smtp = new SmtpClient
        {
            Host = _options.Host,
            Port = _options.Port,
            EnableSsl = _options.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(username, _options.Password)
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        await smtp.SendMailAsync(message, cancellationToken);
    }

    private void ValidateConfiguration()
    {
        var missingSettings = new List<string>();

        if (string.IsNullOrWhiteSpace(_options.FromAddress))
            missingSettings.Add("Email:FromAddress");
        if (string.IsNullOrWhiteSpace(_options.Host))
            missingSettings.Add("Email:Host");
        if (string.IsNullOrWhiteSpace(_options.Password))
            missingSettings.Add("Email:Password");

        if (missingSettings.Count > 0)
        {
            throw new InvalidOperationException(
                $"SMTP is not configured. Missing: {string.Join(", ", missingSettings)}.");
        }
    }
}
