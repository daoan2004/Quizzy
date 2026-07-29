using System.Net;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class RegistrationOwnershipTests
{
    [Theory]
    [InlineData("/api/MyRegistrationsApi/GetAllRegistrations/92002")]
    [InlineData("/api/SimulationExamApi/GetExamPagination/92002")]
    [InlineData("/api/SimulationExamApi/LoadFilter/92002")]
    public async Task List_endpoints_ignore_user_id_supplied_by_client(string route)
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await AddRegistrationOwnedByAnotherUserAsync(session.Factory);

        using var response = await session.Client.GetAsync(route);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("92001", body, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("/api/MyRegistrationsApi/CancelRegistration/92001")]
    [InlineData("/api/MyRegistrationsApi/PayPackage/92001")]
    public async Task User_cannot_mutate_another_users_registration(string route)
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await AddRegistrationOwnedByAnotherUserAsync(session.Factory);

        using var response = await session.Client.PostAsync(route, content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        Assert.Equal("Submitted", context.Recipe.Single(r => r.ID == 92001).Status);
    }

    [Theory]
    [InlineData("/SimulationExam")]
    [InlineData("/MyRegistrations")]
    [InlineData("/api/SimulationExamApi/GetExamPagination/1")]
    [InlineData("/api/MyRegistrationsApi/GetAllRegistrations/1")]
    public async Task Guest_cannot_access_private_learning_routes(string route)
    {
        await using var factory = new QuizzyWebApplicationFactory();
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync(route);

        if (route.StartsWith("/api/", StringComparison.Ordinal))
        {
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        else
        {
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Account/Login", response.Headers.Location?.AbsolutePath);
        }
    }

    private static async Task AddRegistrationOwnedByAnotherUserAsync(
        QuizzyWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Recipe.Add(new RecipeModel
        {
            ID = 92001,
            PricePackage_ID = 1,
            UserID = 92002,
            SubjectID = 1,
            PricePackage_Type = 1,
            BuyAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddDays(30),
            Status = "Submitted"
        });
        await context.SaveChangesAsync();
    }
}
