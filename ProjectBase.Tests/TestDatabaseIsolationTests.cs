using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class TestDatabaseIsolationTests
{
    [Fact]
    public void Factories_use_isolated_in_memory_databases()
    {
        using var firstFactory = new QuizzyWebApplicationFactory();
        using (var firstScope = firstFactory.Services.CreateScope())
        {
            var firstContext = firstScope.ServiceProvider.GetRequiredService<DataContext>();
            Assert.Equal("Microsoft.EntityFrameworkCore.InMemory", firstContext.Database.ProviderName);

            firstContext.Category.Add(new CategoryModel
            {
                title = "Isolation marker",
                description = "Must not appear in another test factory"
            });
            firstContext.SaveChanges();
            Assert.Single(firstContext.Category);
        }

        using var secondFactory = new QuizzyWebApplicationFactory();
        using var secondScope = secondFactory.Services.CreateScope();
        var secondContext = secondScope.ServiceProvider.GetRequiredService<DataContext>();

        Assert.Equal("Microsoft.EntityFrameworkCore.InMemory", secondContext.Database.ProviderName);
        Assert.Empty(secondContext.Category);
    }

    [Fact]
    public async Task Reset_removes_mutated_test_data_and_recreates_seed_data()
    {
        using var factory = new QuizzyWebApplicationFactory();
        using (var mutationScope = factory.Services.CreateScope())
        {
            var context = mutationScope.ServiceProvider.GetRequiredService<DataContext>();
            context.Category.Add(new CategoryModel
            {
                title = "Temporary category",
                description = "Removed by reset"
            });
            await context.SaveChangesAsync();
            Assert.Single(context.Category);
        }

        await factory.ResetDatabaseAsync();

        using var verificationScope = factory.Services.CreateScope();
        var resetContext = verificationScope.ServiceProvider.GetRequiredService<DataContext>();
        Assert.Empty(resetContext.Category);
        Assert.Equal(6, resetContext.Set<Role>().Count());
    }
}
