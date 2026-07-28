using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ProjectBase.Models.DAO;
using ProjectBase.Services;

namespace ProjectBase.Tests;

public sealed class PasswordServiceTests
{
    private readonly PasswordService _service = new(new PasswordHasher<User>());

    [Fact]
    public void Hash_and_verify_new_password()
    {
        var user = CreateUser();
        user.password = _service.HashPassword(user, "Customer@123");

        var result = _service.VerifyPassword(user, "Customer@123");

        Assert.True(result.Succeeded);
        Assert.False(result.NeedsRehash);
        Assert.NotEqual(LegacyMd5("Customer@123"), user.password);
    }

    [Fact]
    public void Same_password_has_different_salted_hashes()
    {
        var first = CreateUser();
        var second = CreateUser();

        first.password = _service.HashPassword(first, "Customer@123");
        second.password = _service.HashPassword(second, "Customer@123");

        Assert.NotEqual(first.password, second.password);
    }

    [Fact]
    public void Valid_legacy_md5_requests_rehash()
    {
        var user = CreateUser();
        user.password = LegacyMd5("Legacy@123");

        var result = _service.VerifyPassword(user, "Legacy@123");

        Assert.True(result.Succeeded);
        Assert.True(result.NeedsRehash);
    }

    [Fact]
    public void Wrong_password_is_rejected()
    {
        var user = CreateUser();
        user.password = _service.HashPassword(user, "Customer@123");

        Assert.False(_service.VerifyPassword(user, "Wrong@123").Succeeded);
    }

    private static User CreateUser() => new()
    {
        email = "password-test@quizzy.test",
        fullname = "Password Test",
        password = string.Empty,
        Phone = "0901234567"
    };

    internal static string LegacyMd5(string password) =>
        Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(password))).ToLowerInvariant();
}
