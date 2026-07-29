using System.Net.Mail;

namespace ProjectBase.Configuration;

public static class ProductionConfigurationValidator
{
    public static void Validate(IConfiguration configuration)
    {
        var errors = GetErrors(configuration);
        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            "Production configuration is invalid. " +
            string.Join(" ", errors));
    }

    public static IReadOnlyList<string> GetErrors(IConfiguration configuration)
    {
        var errors = new List<string>();

        Require(configuration, "ConnectionStrings:ConnectedDb", errors);
        Require(configuration, "Email:FromAddress", errors);
        Require(configuration, "Email:Host", errors);
        Require(configuration, "Email:Password", errors);
        Require(configuration, "Email:BaseUrl", errors);

        var fromAddress = configuration["Email:FromAddress"];
        if (!string.IsNullOrWhiteSpace(fromAddress) && !IsEmailAddress(fromAddress))
        {
            errors.Add("Email:FromAddress must be a valid email address.");
        }

        var baseUrl = configuration["Email:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(baseUrl) &&
            (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) ||
             (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
        {
            errors.Add("Email:BaseUrl must be an absolute HTTP or HTTPS URL.");
        }

        if (!int.TryParse(configuration["Email:Port"], out var port) ||
            port is < 1 or > 65535)
        {
            errors.Add("Email:Port must be between 1 and 65535.");
        }

        RequirePositiveInteger(
            configuration,
            "PasswordResetLinkExpirationHours",
            errors);
        RequirePositiveInteger(
            configuration,
            "VerificationLinkExpirationHours",
            errors);

        return errors;
    }

    private static void Require(
        IConfiguration configuration,
        string key,
        ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(configuration[key]))
        {
            errors.Add($"{key} is required.");
        }
    }

    private static void RequirePositiveInteger(
        IConfiguration configuration,
        string key,
        ICollection<string> errors)
    {
        if (!int.TryParse(configuration[key], out var value) || value <= 0)
        {
            errors.Add($"{key} must be greater than zero.");
        }
    }

    private static bool IsEmailAddress(string value)
    {
        try
        {
            return new MailAddress(value).Address == value;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
