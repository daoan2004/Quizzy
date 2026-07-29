using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Tests;

public sealed class ApiErrorResponseTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly string _controllersPath;

    public ApiErrorResponseTests(QuizzyWebApplicationFactory factory)
    {
        var contentRoot = factory.Services
            .GetRequiredService<IWebHostEnvironment>()
            .ContentRootPath;
        _controllersPath = Path.Combine(contentRoot, "Controllers");
    }

    [Fact]
    public void Controllers_do_not_return_exception_messages_to_clients()
    {
        var unsafeLines = Directory
            .EnumerateFiles(_controllersPath, "*.cs")
            .SelectMany(path => File.ReadLines(path)
                .Select((line, index) => new { path, line, index }))
            .Where(item =>
                item.line.Contains("return", StringComparison.Ordinal) &&
                item.line.Contains("ex.Message", StringComparison.Ordinal))
            .Select(item => $"{Path.GetFileName(item.path)}:{item.index + 1}")
            .ToList();

        Assert.Empty(unsafeLines);
    }

    [Fact]
    public void Private_api_controllers_use_problem_details_for_server_failures()
    {
        foreach (var fileName in new[]
                 {
                     "MyRegistrationsApiController.cs",
                     "PracticeApiController.cs",
                     "SimulationExamApiController.cs"
                 })
        {
            var source = File.ReadAllText(Path.Combine(_controllersPath, fileName));
            Assert.Contains("return Problem(", source, StringComparison.Ordinal);
            Assert.Contains(
                "StatusCodes.Status500InternalServerError",
                source,
                StringComparison.Ordinal);
        }
    }
}
