using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;
using ProjectBase.Services;

namespace ProjectBase.Tests;

public sealed class SmtpUnavailableIntegrationTests
{
    [Fact]
    public async Task Registration_failure_removes_the_pending_user()
    {
        await using var factory = new SmtpUnavailableFactory();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonWithCsrfAsync(
            "/Account/Register",
            new
            {
                fullname = "SMTP Failure",
                password = "Customer@123",
                confirmPassword = "Customer@123",
                email = "smtp-failure@quizzy.test",
                phone = "0901234567",
                gender = true
            });
        using var payload = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(
            "Unable to send verification email.",
            payload.RootElement.GetProperty("title").GetString());

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        Assert.False(await context.Users.AnyAsync(
            user => user.email == "smtp-failure@quizzy.test"));
    }

    [Fact]
    public async Task Password_reset_failure_clears_the_generated_token()
    {
        await using var factory = new SmtpUnavailableFactory();
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Users.Add(new User
            {
                ID = 97001,
                email = "smtp-reset@quizzy.test",
                fullname = "SMTP Reset",
                password = "not-used",
                Phone = "0901234567",
                gender = true,
                RoleID = 2,
                status = 1
            });
            await context.SaveChangesAsync();
        }

        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonWithCsrfAsync(
            "/Account/ResetPasswordRequest",
            new { email = "smtp-reset@quizzy.test" });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        using var verificationScope = factory.Services.CreateScope();
        var verificationContext =
            verificationScope.ServiceProvider.GetRequiredService<DataContext>();
        var user = await verificationContext.Users.SingleAsync(
            item => item.ID == 97001);
        Assert.Null(user.PasswordResetToken);
        Assert.Null(user.PasswordResetTokenExpires);
    }

    private sealed class SmtpUnavailableFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = $"SmtpUnavailable-{Guid.NewGuid()}";

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
                services.AddSingleton<IEmailSender, UnavailableEmailSender>();

                using var scope = services.BuildServiceProvider().CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                context.Database.EnsureCreated();
            });
        }
    }

    private sealed class UnavailableEmailSender : IEmailSender
    {
        public Task SendVerificationLinkAsync(
            string recipient,
            string token,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("Simulated SMTP outage.");

        public Task SendPasswordResetLinkAsync(
            string recipient,
            string token,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("Simulated SMTP outage.");
    }
}
