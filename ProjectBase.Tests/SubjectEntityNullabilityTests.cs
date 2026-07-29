using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class SubjectEntityNullabilityTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;

    public SubjectEntityNullabilityTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Ef_metadata_preserves_required_and_optional_subject_columns()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var entity = context.Model.FindEntityType(typeof(SubjectsModel));

        Assert.NotNull(entity);
        Assert.False(Property(entity, nameof(SubjectsModel.title)).IsNullable);
        Assert.False(Property(entity, nameof(SubjectsModel.brief_info)).IsNullable);
        Assert.False(Property(entity, nameof(SubjectsModel.Description)).IsNullable);
        Assert.True(Property(entity, nameof(SubjectsModel.thumbnail_color)).IsNullable);
        Assert.True(Property(entity, nameof(SubjectsModel.registerDate)).IsNullable);
    }

    [Fact]
    public void New_aggregate_collections_are_initialized()
    {
        var subject = new SubjectsModel();
        var category = new CategoryModel();
        var package = new PricePackageModel();

        Assert.Empty(subject.Subject_Category);
        Assert.Empty(subject.Price_package);
        Assert.Empty(subject.Practice);
        Assert.Empty(subject.Recipes);
        Assert.Empty(subject.Exams);
        Assert.Empty(category.Subject_Category);
        Assert.Empty(package.Recipe);
    }

    private static IProperty Property(IEntityType entity, string name) =>
        entity.FindProperty(name)
        ?? throw new InvalidOperationException($"EF property '{name}' was not found.");
}
