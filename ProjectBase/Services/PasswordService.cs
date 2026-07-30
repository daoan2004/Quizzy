using Microsoft.AspNetCore.Identity;
using ProjectBase.Models.DAO;

namespace ProjectBase.Services;

public sealed class PasswordService : IPasswordService
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

}
