using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class QuizEntityNullabilityTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;

    public QuizEntityNullabilityTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Ef_metadata_keeps_quiz_text_columns_required()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        AssertRequired(context.Model.FindEntityType(typeof(QuizBankModel)), nameof(QuizBankModel.Title));
        AssertRequired(context.Model.FindEntityType(typeof(QuizBankModel)), nameof(QuizBankModel.Qcorrect));
        AssertRequired(context.Model.FindEntityType(typeof(QuizHandleModel)), nameof(QuizHandleModel.QAnswer));
        AssertRequired(context.Model.FindEntityType(typeof(SimulationExam)), nameof(SimulationExam.ExamName));
    }

    [Fact]
    public void Quiz_bank_collection_is_initialized()
    {
        Assert.Empty(new QuizBankModel().QuizHandle);
    }

    private static void AssertRequired(IEntityType? entity, string propertyName)
    {
        Assert.NotNull(entity);
        var property = entity.FindProperty(propertyName);
        Assert.NotNull(property);
        Assert.False(property.IsNullable);
    }
}
