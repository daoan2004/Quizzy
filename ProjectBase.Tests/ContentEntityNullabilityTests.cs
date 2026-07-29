using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class ContentEntityNullabilityTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;

    public ContentEntityNullabilityTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData(typeof(BlogsModel), nameof(BlogsModel.title))]
    [InlineData(typeof(BlogsModel), nameof(BlogsModel.body))]
    [InlineData(typeof(RecipeModel), nameof(RecipeModel.Status))]
    [InlineData(typeof(SliderModel), nameof(SliderModel.Title))]
    [InlineData(typeof(SliderModel), nameof(SliderModel.image))]
    public void Ef_metadata_keeps_content_columns_required(Type entityType, string propertyName)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        IEntityType? entity = context.Model.FindEntityType(entityType);

        Assert.NotNull(entity);
        var property = entity.FindProperty(propertyName);
        Assert.NotNull(property);
        Assert.False(property.IsNullable);
    }
}
