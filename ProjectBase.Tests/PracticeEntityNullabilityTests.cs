using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class PracticeEntityNullabilityTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;

    public PracticeEntityNullabilityTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Practice_lookup_seed_can_be_read()
    {
        await _factory.ResetDatabaseAsync();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        Assert.Equal(3, context.PracticeLevel.Count());
        Assert.Contains(context.PracticeLevel, level => level.title == "Easy");
        Assert.Contains(context.SubjectTopic, topic => topic.title == "General");
    }

    [Fact]
    public void Collections_are_initialized()
    {
        var level = new PracticeLevel();
        var topic = new SubjectTopicModel();

        Assert.Empty(level.Practice);
        Assert.Empty(level.Exams);
        Assert.Empty(topic.Practice);
    }
}
