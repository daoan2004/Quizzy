using System.Net;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;
using System.Text.RegularExpressions;

namespace ProjectBase.Tests;

public sealed class SubjectRegistrationOwnershipTests
{
    [Fact]
    public async Task Registration_uses_authenticated_user_instead_of_posted_user_id()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await AddSubjectPackageAsync(session.Factory);
        var antiForgeryToken = await GetAntiForgeryTokenAsync(session.Client);
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["subjectId"] = "93001",
            ["userId"] = "93002",
            ["selectedPackage"] = "1",
            ["packageId"] = "93003",
            ["__RequestVerificationToken"] = antiForgeryToken
        });

        using var response = await session.Client.PostAsync(
            "/SubjectRegister/Register",
            content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var registration = Assert.Single(context.Recipe);
        Assert.Equal(session.UserId, registration.UserID);
        Assert.NotEqual(93002, registration.UserID);
    }

    [Fact]
    public async Task Authenticated_registration_without_anti_forgery_token_is_rejected()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["subjectId"] = "93001",
            ["userId"] = session.UserId.ToString(),
            ["selectedPackage"] = "1",
            ["packageId"] = "93003"
        });

        using var response = await session.Client.PostAsync(
            "/SubjectRegister/Register",
            content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        Assert.Empty(context.Recipe);
    }

    [Fact]
    public async Task Guest_cannot_create_subject_registration()
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.PostAsync(
            "/SubjectRegister/Register",
            new FormUrlEncodedContent(new Dictionary<string, string>()));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/Account/Login", response.Headers.Location?.AbsolutePath);
    }

    private static async Task AddSubjectPackageAsync(QuizzyWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var subject = new SubjectsModel
        {
            ID = 93001,
            title = "CSRF Test Subject",
            brief_info = "Test",
            Description = "Test",
            rate = 5
        };
        context.Subjects.Add(subject);
        context.Price_package.Add(new PricePackageModel
        {
            ID = 93003,
            SubjectID = subject.ID,
            PackageType = 1,
            ListPrice = 10,
            SalePrice = 8
        });
        await context.SaveChangesAsync();
    }

    private static async Task<string> GetAntiForgeryTokenAsync(HttpClient client)
    {
        using var response = await client.PostAsync(
            "/Subjects/GetSubjectData?subjectId=93001&userId=93002",
            content: null);
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(
            html,
            "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(match.Success, "The registration form did not render an anti-forgery token.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }
}
