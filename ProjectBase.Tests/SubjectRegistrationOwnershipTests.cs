using System.Net;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;

namespace ProjectBase.Tests;

public sealed class SubjectRegistrationOwnershipTests
{
    [Fact]
    public async Task Registration_uses_authenticated_user_instead_of_posted_user_id()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["subjectId"] = "93001",
            ["userId"] = "93002",
            ["selectedPackage"] = "1",
            ["packageId"] = "93003"
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
}
