using System.ComponentModel.DataAnnotations;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class RequestModelValidationTests
{
    [Fact]
    public void Login_rejects_missing_and_invalid_values()
    {
        Assert.Contains(nameof(LoginModel.email), InvalidMembers(new LoginModel()));
        Assert.Contains(
            nameof(LoginModel.email),
            InvalidMembers(new LoginModel { email = "not-an-email", password = "Password@123" }));
    }

    [Fact]
    public void Register_rejects_missing_mismatched_and_invalid_values()
    {
        var invalid = new RegisterModel
        {
            fullname = "Test User",
            email = "invalid-email",
            password = "Password@123",
            ConfirmPassword = "Different@123",
            Phone = "123"
        };

        var members = InvalidMembers(invalid);

        Assert.Contains(nameof(RegisterModel.email), members);
        Assert.Contains(nameof(RegisterModel.ConfirmPassword), members);
        Assert.Contains(nameof(RegisterModel.Phone), members);
    }

    [Fact]
    public void Change_password_rejects_missing_and_mismatched_values()
    {
        var invalid = new ChangePasswordModel
        {
            CurrentPassword = "Current@123",
            NewPassword = "NewPassword@123",
            ConfirmNewPassword = "Different@123"
        };

        Assert.Contains(nameof(ChangePasswordModel.ConfirmNewPassword), InvalidMembers(invalid));
        Assert.NotEmpty(InvalidMembers(new ChangePasswordModel()));
    }

    [Fact]
    public void Reset_password_models_reject_invalid_values()
    {
        Assert.Contains(
            nameof(ResetPasswordRequestModel.email),
            InvalidMembers(new ResetPasswordRequestModel { email = "invalid-email" }));

        var confirm = new ResetPasswordConfirmModel
        {
            NewPassword = "NewPassword@123",
            ReNewPassword = "Different@123",
            Token = string.Empty
        };
        var members = InvalidMembers(confirm);

        Assert.Contains(nameof(ResetPasswordConfirmModel.ReNewPassword), members);
        Assert.Contains(nameof(ResetPasswordConfirmModel.Token), members);
    }

    [Fact]
    public void Profile_requires_name_and_valid_phone_but_allows_optional_fields()
    {
        var invalid = new UpdateUserProfileModel { Phone = "invalid" };
        var invalidMembers = InvalidMembers(invalid);

        Assert.Contains(nameof(UpdateUserProfileModel.FullName), invalidMembers);
        Assert.Contains(nameof(UpdateUserProfileModel.Phone), invalidMembers);

        var valid = new UpdateUserProfileModel
        {
            FullName = "Test User",
            Phone = "0901234567"
        };
        Assert.Empty(Validate(valid));
    }

    private static HashSet<string> InvalidMembers(object model) =>
        Validate(model)
            .SelectMany(result => result.MemberNames)
            .ToHashSet(StringComparer.Ordinal);

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            results,
            validateAllProperties: true);
        return results;
    }
}
