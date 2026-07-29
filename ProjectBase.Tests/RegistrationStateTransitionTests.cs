using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class RegistrationStateTransitionTests
{
    [Fact]
    public async Task Paying_twice_changes_state_once_and_second_request_conflicts()
    {
        await using var session = await CreateSessionWithRegistrationAsync();

        using var first = await session.PostWithAntiForgeryAsync(
            "/api/MyRegistrationsApi/PayPackage/98001", null);
        using var second = await session.PostWithAntiForgeryAsync(
            "/api/MyRegistrationsApi/PayPackage/98001", null);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        Assert.Equal(RegistrationStatuses.Registered, ReadRegistration(session).Status);
    }

    [Fact]
    public async Task Cancelling_twice_changes_state_once_and_second_request_conflicts()
    {
        await using var session = await CreateSessionWithRegistrationAsync();

        using var first = await session.PostWithAntiForgeryAsync(
            "/api/MyRegistrationsApi/CancelRegistration/98001", null);
        using var second = await session.PostWithAntiForgeryAsync(
            "/api/MyRegistrationsApi/CancelRegistration/98001", null);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        Assert.Equal(RegistrationStatuses.Cancelled, ReadRegistration(session).Status);
    }

    [Fact]
    public async Task Changing_package_updates_existing_submitted_registration()
    {
        await using var session = await CreateSessionWithRegistrationAsync();
        using var content = RegistrationForm(packageId: 98003, selectedPackage: 999);

        using var response = await session.PostWithAntiForgeryAsync(
            "/SubjectRegister/Register", content);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        using var scope = session.Factory.Services.CreateScope();
        var registrations = scope.ServiceProvider
            .GetRequiredService<DataContext>().Recipe.ToList();
        var registration = Assert.Single(registrations);
        Assert.Equal(98003, registration.PricePackage_ID);
        Assert.Equal(2, registration.PricePackage_Type);
        Assert.Equal(RegistrationStatuses.Submitted, registration.Status);
    }

    [Fact]
    public async Task Package_from_another_subject_is_rejected_without_database_write()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync();
        await SeedCatalogAsync(session, includeRegistration: false);
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["subjectId"] = "99999",
            ["userId"] = session.UserId.ToString(),
            ["selectedPackage"] = "1",
            ["packageId"] = "98002"
        });

        using var response = await session.PostWithAntiForgeryAsync(
            "/SubjectRegister/Register", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var scope = session.Factory.Services.CreateScope();
        Assert.Empty(scope.ServiceProvider.GetRequiredService<DataContext>().Recipe);
    }

    [Fact]
    public async Task Registration_api_returns_price_from_selected_database_package()
    {
        await using var session = await CreateSessionWithRegistrationAsync();

        using var response = await session.Client.GetAsync(
            $"/api/MyRegistrationsApi/GetAllRegistrations/{session.UserId}");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var registration = Assert.Single(json.RootElement.EnumerateArray());

        Assert.Equal(80, registration.GetProperty("totalCost").GetInt64());
    }

    private static async Task<AuthenticatedTestSession> CreateSessionWithRegistrationAsync()
    {
        var session = await AuthenticatedTestSession.CreateAsync();
        await SeedCatalogAsync(session, includeRegistration: true);
        return session;
    }

    private static async Task SeedCatalogAsync(
        AuthenticatedTestSession session,
        bool includeRegistration)
    {
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var subject = new SubjectsModel
        {
            ID = 98001,
            title = "Registration State Test",
            brief_info = "Test",
            Description = "Test",
            rate = 5
        };
        context.Subjects.Add(subject);
        context.Price_package.AddRange(
            new PricePackageModel
            {
                ID = 98002,
                SubjectID = subject.ID,
                PackageType = 1,
                ListPrice = 100,
                SalePrice = 80
            },
            new PricePackageModel
            {
                ID = 98003,
                SubjectID = subject.ID,
                PackageType = 2,
                ListPrice = 180,
                SalePrice = 140
            });
        if (includeRegistration)
        {
            context.Recipe.Add(new RecipeModel
            {
                ID = 98001,
                PricePackage_ID = 98002,
                UserID = session.UserId,
                SubjectID = subject.ID,
                PricePackage_Type = 1,
                BuyAt = DateTime.UtcNow,
                EndAt = DateTime.UtcNow.AddMonths(3),
                Status = RegistrationStatuses.Submitted
            });
        }
        await context.SaveChangesAsync();
    }

    private static FormUrlEncodedContent RegistrationForm(
        long packageId,
        int selectedPackage) =>
        new(new Dictionary<string, string>
        {
            ["subjectId"] = "98001",
            ["userId"] = "99999",
            ["selectedPackage"] = selectedPackage.ToString(),
            ["packageId"] = packageId.ToString()
        });

    private static RecipeModel ReadRegistration(AuthenticatedTestSession session)
    {
        using var scope = session.Factory.Services.CreateScope();
        return scope.ServiceProvider
            .GetRequiredService<DataContext>()
            .Recipe.Single(registration => registration.ID == 98001);
    }
}
