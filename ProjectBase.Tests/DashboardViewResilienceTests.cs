namespace ProjectBase.Tests;

public class DashboardViewResilienceTests
{
    [Fact]
    public void DashboardView_DefinesEmptyErrorAndUtcChartStates()
    {
        var view = File.ReadAllText(Path.Combine(
            FindProjectRoot(),
            "ProjectBase",
            "Views",
            "Dashboard",
            "Index.cshtml"));

        Assert.Contains("No revenue data for this period.", view);
        Assert.Contains("No reporting data for the selected period.", view);
        Assert.Contains("Failed to load reporting data.", view);
        Assert.Contains("No registrations found.", view);
        Assert.Contains("T00:00:00Z", view);
        Assert.Contains("setUTCDate", view);
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
