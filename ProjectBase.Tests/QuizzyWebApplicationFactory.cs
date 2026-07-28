using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectBase.Helpers;
using ProjectBase.Services;

namespace ProjectBase.Tests;

public sealed class QuizzyWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"QuizzyTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DataContext>();
            services.RemoveAll<DbContextOptions<DataContext>>();

            services.AddDbContext<DataContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.RemoveAll<IEmailSender>();
            services.AddSingleton<FakeEmailSender>();
            services.AddSingleton<IEmailSender>(provider =>
                provider.GetRequiredService<FakeEmailSender>());

            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Database.EnsureCreated();
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
