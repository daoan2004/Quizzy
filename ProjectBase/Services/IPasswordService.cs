using ProjectBase.Models.DAO;

namespace ProjectBase.Services;

public interface IPasswordService
{
    string HashPassword(User user, string password);

    PasswordCheckResult VerifyPassword(User user, string password);
}

public readonly record struct PasswordCheckResult(bool Succeeded, bool NeedsRehash);
