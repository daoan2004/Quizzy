using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Json;
using ProjectBase.Helpers;
using ProjectBase.Models;
using ProjectBase.Models.DAO;
using ProjectBase.Services;

namespace ProjectBase.Tests;

public sealed class PracticeTransactionRollbackTests
{
    [Fact]
    public async Task Failure_after_practice_save_rolls_back_practice_and_handles()
    {
        await using var factory = new RollbackWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        await SeedScenarioAsync(factory);
        var loginToken = await GetCsrfTokenAsync(client);
        using var loginRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
        {
            Content = JsonContent.Create(new
            {
                email = "rollback@quizzy.test",
                password = "Rollback@123"
            })
        };
        loginRequest.Headers.Add("X-CSRF-TOKEN", loginToken);
        using var login = await client.SendAsync(loginRequest);
        login.EnsureSuccessStatusCode();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["SubjectID"] = "99101",
            ["title"] = "Rollback Practice",
            ["number_quest"] = "10",
            ["Quest_group"] = "0",
            ["duration"] = "00:30:00",
            ["levelID"] = "1"
        });
        var csrfToken = await GetCsrfTokenAsync(client);
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/PracticeApi/AddPractice")
        {
            Content = content
        };
        request.Headers.Add("X-CSRF-TOKEN", csrfToken);

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        Assert.Empty(await context.Practice.AsNoTracking().ToListAsync());
        Assert.Empty(await context.QuizHandle.AsNoTracking().ToListAsync());
    }

    private static async Task SeedScenarioAsync(RollbackWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var user = new User
        {
            ID = 99101,
            email = "rollback@quizzy.test",
            fullname = "Rollback Test",
            password = string.Empty,
            Phone = "0901234567",
            gender = true,
            RoleID = 2,
            status = 1
        };
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        user.password = passwordService.HashPassword(user, "Rollback@123");
        context.Users.Add(user);
        context.Subjects.Add(new SubjectsModel
        {
            ID = 99101,
            title = "Rollback Subject",
            brief_info = "Test",
            Description = "Test",
            rate = 5
        });
        context.Price_package.Add(new PricePackageModel
        {
            ID = 99101,
            SubjectID = 99101,
            PackageType = 1,
            ListPrice = 100,
            SalePrice = 80
        });
        context.Recipe.Add(new RecipeModel
        {
            ID = 99101,
            PricePackage_ID = 99101,
            UserID = user.ID!.Value,
            SubjectID = 99101,
            PricePackage_Type = 1,
            BuyAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddMonths(3),
            Status = RegistrationStatuses.Registered
        });
        for (var index = 1; index <= 9; index++)
        {
            context.QuizBank.Add(Question(99100 + index, level: 1));
        }
        context.QuizBank.Add(Question(99201, level: 2));
        await context.SaveChangesAsync();
    }

    private static QuizBankModel Question(long id, int level) => new()
    {
        ID = id,
        SubjectID = 99101,
        TopicID = 1,
        LevelID = level,
        Status = true,
        GroupID = "A",
        Title = $"Question {id}",
        QA = "A",
        QB = "B",
        QC = "C",
        QD = "D",
        QE = "E",
        QF = "F",
        Qcorrect = "A"
    };

    private static async Task<string> GetCsrfTokenAsync(HttpClient client)
    {
        using var response = await client.GetAsync("/Account/ResetPasswordRequest");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        var marker = "<meta name=\"csrf-token\" content=\"";
        var start = html.IndexOf(marker, StringComparison.Ordinal) + marker.Length;
        var end = html.IndexOf('"', start);
        return System.Net.WebUtility.HtmlDecode(html[start..end]);
    }

    private sealed class ThrowAfterPracticeSaved : IPracticeCreationFaultInjector
    {
        public Task AfterPracticeSavedAsync(CancellationToken cancellationToken) =>
            throw new InvalidOperationException("Injected failure after Practice save.");
    }

    private sealed class RollbackWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection = new("Data Source=:memory:");

        public RollbackWebApplicationFactory()
        {
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DataContext>();
                services.RemoveAll<DbContextOptions<DataContext>>();
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlite(_connection));
                services.RemoveAll<IPracticeCreationFaultInjector>();
                services.AddScoped<IPracticeCreationFaultInjector, ThrowAfterPracticeSaved>();
                services.RemoveAll<IEmailSender>();
                services.AddSingleton<IEmailSender, FakeEmailSender>();

                using var scope = services.BuildServiceProvider().CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                context.Database.EnsureCreated();
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection.Dispose();
            }
        }
    }
}
