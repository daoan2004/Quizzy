namespace ProjectBase.Tests;

public class DashboardViewResilienceTests
{
    [Fact]
    public void DashboardView_DefinesEmptyErrorAndUtcChartStates()
    {
        var root = FindProjectRoot();
        var view = File.ReadAllText(Path.Combine(
            root, "ProjectBase", "Views", "Dashboard", "Index.cshtml"));
        var script = File.ReadAllText(Path.Combine(
            root, "ProjectBase", "wwwroot", "js", "Dashboard.js"));

        Assert.Contains("src=\"~/js/Dashboard.js\"", view);
        Assert.DoesNotContain("<script>\n", view);
        Assert.Contains("No revenue data for this period.", script);
        Assert.Contains("No reporting data for the selected period.", script);
        Assert.Contains("Failed to load reporting data.", script);
        Assert.Contains("No registrations found.", script);
        Assert.Contains("T00:00:00Z", script);
        Assert.Contains("setUTCDate", script);
    }

    private static string FindProjectRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "SWP391.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Could not locate the solution root.");
    }
}
