using Microsoft.Extensions.Options;

namespace ProjectBase.Services;

public sealed class AccountLinkBuilder : IAccountLinkBuilder
{
    private readonly EmailOptions _options;

    public AccountLinkBuilder(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public string BuildVerificationLink(string token) =>
        BuildLink("Account/VerifyAccount", token);

    public string BuildPasswordResetLink(string token) =>
        BuildLink("Account/ResetPasswordConfirm", token);

    private string BuildLink(string path, string token)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl) ||
            !Uri.TryCreate(_options.BaseUrl, UriKind.Absolute, out var baseUri))
        {
            throw new InvalidOperationException(
                "Email:BaseUrl must be configured as an absolute URL.");
        }

        var normalizedBaseUri = new Uri($"{baseUri.ToString().TrimEnd('/')}/");
        var encodedToken = Uri.EscapeDataString(token);
        return new Uri(normalizedBaseUri, $"{path}?token={encodedToken}").ToString();
    }
}
