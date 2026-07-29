using System.Text.RegularExpressions;

namespace ProjectBase.Tests;

public class FrontendCssQualityTests
{
    [Fact]
    public void PageStyles_DoNotSuppressKeyboardFocusOutlines()
    {
        var cssRoot = Path.Combine(FindProjectRoot(), "ProjectBase", "wwwroot", "css");
        var offenders = Directory
            .EnumerateFiles(cssRoot, "*.css")
            .Where(path => Regex.IsMatch(
                File.ReadAllText(path),
                @"outline\s*:\s*(none|0)\s*;",
                RegexOptions.IgnoreCase))
            .Select(Path.GetFileName)
            .ToArray();

        Assert.Empty(offenders);
    }

    [Fact]
    public void ResetPasswordStyles_DoNotRequireImportantOverrides()
    {
        var css = File.ReadAllText(Path.Combine(
            FindProjectRoot(),
            "ProjectBase",
            "wwwroot",
            "css",
            "ResetPassword.css"));

        Assert.DoesNotContain("!important", css, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DashboardStyles_DoNotReintroduceVerifiedLegacyOrDuplicateSelectors()
    {
        var css = File.ReadAllText(Path.Combine(
            FindProjectRoot(),
            "ProjectBase",
            "wwwroot",
            "css",
            "Dashboard.css"));

        Assert.DoesNotContain(".dashboard-error", css);
        Assert.DoesNotContain(".customer-stat-item", css);
        Assert.DoesNotContain(".customer-stat-title", css);
        Assert.Single(Regex.Matches(css, @"(?m)^canvas\s*\{"));
    }

    [Fact]
    public void SharedStyles_DefineAConsistentDisabledButtonState()
    {
        var css = File.ReadAllText(Path.Combine(
            FindProjectRoot(),
            "ProjectBase",
            "wwwroot",
            "css",
            "site.css"));

        Assert.Contains("):disabled", css);
        Assert.Contains("cursor: not-allowed", css);
        Assert.Contains("opacity: 0.56", css);
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
