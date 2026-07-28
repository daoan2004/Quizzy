using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using ProjectBase.Models.DAO;

namespace ProjectBase.Services;

public sealed partial class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(User user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrEmpty(password);

        return _passwordHasher.HashPassword(user, password);
    }

    public PasswordCheckResult VerifyPassword(User user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(user.password))
        {
            return new PasswordCheckResult(false, false);
        }

        if (LegacyMd5Pattern().IsMatch(user.password))
        {
            var suppliedHash = MD5.HashData(Encoding.UTF8.GetBytes(password));
            var storedHash = Convert.FromHexString(user.password);
            var succeeded = CryptographicOperations.FixedTimeEquals(suppliedHash, storedHash);

            return new PasswordCheckResult(succeeded, succeeded);
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.password, password);
        return result switch
        {
            PasswordVerificationResult.Success =>
                new PasswordCheckResult(true, false),
            PasswordVerificationResult.SuccessRehashNeeded =>
                new PasswordCheckResult(true, true),
            _ => new PasswordCheckResult(false, false)
        };
    }

    [GeneratedRegex(@"\A[0-9a-fA-F]{32}\z", RegexOptions.CultureInvariant)]
    private static partial Regex LegacyMd5Pattern();
}
