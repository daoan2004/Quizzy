using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;

namespace ProjectBase.Tests;

public sealed class UserRoleEntityTests : IClassFixture<QuizzyWebApplicationFactory>
{
    private readonly QuizzyWebApplicationFactory _factory;

    public UserRoleEntityTests(QuizzyWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Lookup_role_seed_can_be_read()
    {
        await _factory.ResetDatabaseAsync();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var roles = context.Set<Role>().OrderBy(role => role.RoleID).ToList();

        Assert.Equal(6, roles.Count);
        Assert.Equal("Admin", roles[0].RoleName);
        Assert.Equal("Customer", roles[1].RoleName);
    }

    [Fact]
    public void New_entity_collections_are_initialized()
    {
        var user = new User();
        var role = new Role();

        Assert.Empty(user.Practice);
        Assert.Empty(user.Recipes);
        Assert.Empty(user.Sliders);
        Assert.Empty(role.Users);
    }
}
