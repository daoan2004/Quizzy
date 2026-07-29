using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public class DashboardReportingIntegrationTests
{
    [Fact]
    public async Task DateRangeEndpoints_ValidateRequiredOrderAndMaximumLength()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync("Marketing");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            (await session.Client.GetAsync("/api/dashboardapi/order-count")).StatusCode);
        Assert.Equal(
            HttpStatusCode.BadRequest,
            (await session.Client.GetAsync(
                "/api/dashboardapi/order-count?startDate=2026-07-30T00:00:00Z&endDate=2026-07-29T00:00:00Z")).StatusCode);
        Assert.Equal(
            HttpStatusCode.BadRequest,
            (await session.Client.GetAsync(
                "/api/dashboardapi/registration-count?startDate=2025-01-01T00:00:00Z&endDate=2026-07-29T00:00:00Z")).StatusCode);
    }

    [Fact]
    public async Task ReportingEndpoints_MatchRegisteredRevenueAndUtcDateRange()
    {
        await using var session = await CreateReportingDataAsync();

        using var totalResponse = await session.Client.GetAsync("/api/dashboardapi/total-revenue");
        using var subjectResponse = await session.Client.GetAsync(
            "/api/dashboardapi/RevenuesBySubject?subjectId=99701");
        using var subjectsResponse = await session.Client.GetAsync(
            "/api/dashboardapi/revenues-by-subject");
        using var orderResponse = await session.Client.GetAsync(
            "/api/dashboardapi/order-count?startDate=2026-07-28T17:00:00%2B07:00&endDate=2026-07-29T17:00:00%2B07:00");
        using var registrationResponse = await session.Client.GetAsync(
            "/api/dashboardapi/registration-count?startDate=2026-07-28T17:00:00%2B07:00&endDate=2026-07-29T17:00:00%2B07:00");

        totalResponse.EnsureSuccessStatusCode();
        subjectResponse.EnsureSuccessStatusCode();
        subjectsResponse.EnsureSuccessStatusCode();
        orderResponse.EnsureSuccessStatusCode();
        registrationResponse.EnsureSuccessStatusCode();

        using var total = JsonDocument.Parse(await totalResponse.Content.ReadAsStringAsync());
        using var subjectRevenue = JsonDocument.Parse(await subjectResponse.Content.ReadAsStringAsync());
        using var subjectRevenues = JsonDocument.Parse(await subjectsResponse.Content.ReadAsStringAsync());
        using var orders = JsonDocument.Parse(await orderResponse.Content.ReadAsStringAsync());
        using var registrations = JsonDocument.Parse(await registrationResponse.Content.ReadAsStringAsync());
        Assert.Equal(125_000, total.RootElement.GetProperty("totalRevenue").GetInt64());
        Assert.Equal(125_000, subjectRevenue.RootElement.GetProperty("totalRevenue").GetInt64());
        Assert.Contains(
            subjectRevenues.RootElement.EnumerateArray(),
            item => item.GetProperty("subjectName").GetString() == "Reporting Subject" &&
                    item.GetProperty("revenue").GetInt64() == 125_000);
        Assert.Equal(2, orders.RootElement[0].GetProperty("count").GetInt32());
        Assert.Equal(1, registrations.RootElement[0].GetProperty("count").GetInt32());
    }

    [Fact]
    public async Task EmptyDateRange_ReturnsEmptyDatasets()
    {
        await using var session = await AuthenticatedTestSession.CreateAsync("Admin");

        using var response = await session.Client.GetAsync(
            "/api/dashboardapi/order-count?startDate=2035-01-01T00:00:00Z&endDate=2035-01-07T00:00:00Z");
        var json = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("[]", json);
    }

    private static async Task<AuthenticatedTestSession> CreateReportingDataAsync()
    {
        var session = await AuthenticatedTestSession.CreateAsync("Marketing");
        using var scope = session.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var subject = new SubjectsModel
        {
            ID = 99701,
            title = "Reporting Subject",
            brief_info = "Reporting",
            Description = "Reporting",
            rate = 5
        };
        var package = new PricePackageModel
        {
            ID = 99701,
            SubjectID = subject.ID,
            PackageType = 1,
            ListPrice = 150_000,
            SalePrice = 125_000
        };
        context.Subjects.Add(subject);
        context.Price_package.Add(package);
        context.Recipe.AddRange(
            CreateRecipe(99701, session.UserId, subject.ID, package.ID, RegistrationStatuses.Registered),
            CreateRecipe(99702, session.UserId, subject.ID, package.ID, RegistrationStatuses.Submitted));
        await context.SaveChangesAsync();
        return session;
    }

    private static RecipeModel CreateRecipe(
        long id,
        long userId,
        long subjectId,
        long packageId,
        string status) =>
        new()
        {
            ID = id,
            UserID = userId,
            SubjectID = subjectId,
            PricePackage_ID = packageId,
            PricePackage_Type = 1,
            BuyAt = new DateTime(2026, 7, 29, 12, 0, 0, DateTimeKind.Utc),
            EndAt = new DateTime(2026, 8, 29, 12, 0, 0, DateTimeKind.Utc),
            Status = status
        };
}
