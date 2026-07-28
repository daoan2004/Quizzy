namespace ProjectBase.Services;

public interface IAccountLinkBuilder
{
    string BuildVerificationLink(string token);
    string BuildPasswordResetLink(string token);
}
