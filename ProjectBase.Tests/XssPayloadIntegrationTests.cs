using System.Net;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Tests;

public sealed class XssPayloadIntegrationTests
{
    [Fact]
    public async Task Subject_partial_encodes_script_payload_from_database()
    {
        const string payload = "<script>window.quizlyXss=true</script>";
        await using var session = await AuthenticatedTestSession.CreateAsync();

        using (var scope = session.Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Subjects.Add(new SubjectsModel
            {
                ID = 94001,
                title = payload,
                brief_info = payload,
                Description = payload,
                rate = 1
            });
            await context.SaveChangesAsync();
        }

        using var response = await session.Client.PostAsync(
            "/Subjects/GetSubjectData?subjectId=94001&userId=99999",
            content: null);
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(payload, html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            "<script>window.quizlyXss=true</script>",
            html,
            StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            "&lt;script&gt;window.quizlyXss=true&lt;/script&gt;",
            html,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Script_payload_does_not_change_the_authenticated_user_context()
    {
        const string payload = "<img src=x onerror=alert(1)>";
        await using var session = await AuthenticatedTestSession.CreateAsync();

        using (var scope = session.Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Subjects.Add(new SubjectsModel
            {
                ID = 94002,
                title = payload,
                brief_info = "Safe brief",
                Description = "Safe description",
                rate = 1
            });
            await context.SaveChangesAsync();
        }

        using var response = await session.Client.PostAsync(
            "/Subjects/GetSubjectData?subjectId=94002&userId=99999",
            content: null);
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(payload, html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("&lt;img src=x onerror=alert(1)&gt;", html);
        Assert.Contains("Register Now", html, StringComparison.Ordinal);
        Assert.DoesNotContain("value=\"99999\"", html);
    }
}
