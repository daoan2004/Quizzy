namespace ProjectBase.Tests;

public class AccessibilityMarkupTests
{
    [Fact]
    public void SharedLayout_HasKeyboardSkipLinkFocusTargetAndReducedMotion()
    {
        var layout = Read("Views", "Shared", "_Layout.cshtml");
        var siteCss = Read("wwwroot", "css", "site.css");

        Assert.Contains("class=\"skip-link\" href=\"#main-content\"", layout);
        Assert.Contains("id=\"main-content\"", layout);
        Assert.Contains("tabindex=\"-1\"", layout);
        Assert.Contains(":focus-visible", siteCss);
        Assert.Contains("@media (prefers-reduced-motion: reduce)", siteCss);
    }

    [Fact]
    public void Dashboard_DynamicStatesUseLiveRegions()
    {
        var dashboard = Read("Views", "Dashboard", "Index.cshtml");

        Assert.Contains("id=\"registrationList\" class=\"registration-list\" aria-live=\"polite\"", dashboard);
        Assert.Contains("id=\"revenueChartMessage\" class=\"dashboard-message\" role=\"status\"", dashboard);
        Assert.Contains("id=\"error-message\" role=\"alert\" aria-live=\"assertive\"", dashboard);
    }

    [Fact]
    public void QuizReview_NavigationButtonsHaveExplicitTypeAndAccessibleName()
    {
        var review = Read("Views", "QuizReview", "Detail.cshtml");

        Assert.Contains("type=\"button\" id=\"filter-correct\"", review);
        Assert.Contains("aria-label=\"Go to question @index\"", review);
    }

    [Fact]
    public void RegistrationConfirmationModalsExposeDialogSemantics()
    {
        var registrations = Read("Views", "MyRegistrations", "Index.cshtml");

        Assert.Contains("id=\"myModal\" class=\"modal\" role=\"dialog\" aria-modal=\"true\"", registrations);
        Assert.Contains("aria-labelledby=\"cancelModalTitle\"", registrations);
        Assert.Contains("id=\"payModal\" class=\"modal\" role=\"dialog\" aria-modal=\"true\"", registrations);
        Assert.Contains("aria-labelledby=\"payModalTitle\"", registrations);
        Assert.Contains("aria-label=\"Registration pages\"", registrations);
    }

    [Fact]
    public void PracticeViews_ExposeFilterNamesLiveStatesAndExternalScripts()
    {
        var list = Read("Views", "Practice", "Index.cshtml");
        var creation = Read("Views", "Practice", "NewPractice.cshtml");

        Assert.Contains("src=\"~/js/PracticeList.js\"", list);
        Assert.Contains("aria-label=\"Filter by subject\"", list);
        Assert.Contains("aria-label=\"Filter by difficulty\"", list);
        Assert.Contains("id=\"PracticeList\" aria-live=\"polite\"", list);
        Assert.Contains("aria-label=\"Practice pages\"", list);
        Assert.Contains("src=\"~/js/NewPractice.js\"", creation);
        Assert.Contains("for=\"Quest-group\"", creation);
        Assert.Contains("role=\"alert\" aria-live=\"polite\"", creation);
    }

    [Fact]
    public void AccountModals_HaveDialogNamesAndLiveFeedback()
    {
        var login = Read("Views", "Shared", "Login.cshtml");
        var register = Read("Views", "Shared", "Register.cshtml");
        var password = Read("Views", "User", "ChangePassword.cshtml");
        var profile = Read("Views", "User", "Profile.cshtml");

        Assert.Contains("src=\"~/js/AccountLogin.js\"", login);
        Assert.Contains("aria-labelledby=\"loginModalTitle\"", login);
        Assert.Contains("role=\"alert\" aria-live=\"assertive\"", login);
        Assert.Contains("src=\"~/js/AccountRegister.js\"", register);
        Assert.Contains("aria-labelledby=\"registerModalTitle\"", register);
        Assert.Contains("role=\"status\" aria-live=\"polite\"", register);
        Assert.Contains("src=\"~/js/ChangePassword.js\"", password);
        Assert.Contains("aria-label=\"Change password\"", password);
        Assert.Contains("src=\"~/js/Profile.js\"", profile);
        Assert.Contains("aria-labelledby=\"profileModalTitle\"", profile);
        Assert.Contains("id=\"profileModalTitle\"", profile);
    }

    [Fact]
    public void BlogAndSimulationViews_ExposeExternalScriptsAndNavigationNames()
    {
        var blogs = Read("Views", "Blogs", "Index.cshtml");
        var detail = Read("Views", "Blogs", "BlogsDetail.cshtml");
        var simulation = Read("Views", "SimulationExam", "Index.cshtml");

        Assert.Contains("src=\"~/js/BlogIndex.js\"", blogs);
        Assert.Contains("aria-label=\"Blog pages\"", blogs);
        Assert.Contains("src=\"~/js/BlogDetail.js\"", detail);
        Assert.Contains("src=\"~/js/SimulationExam.js\"", simulation);
        Assert.Contains("aria-label=\"Filter simulation exams by level\"", simulation);
        Assert.Contains("id=\"simulationExamList\" aria-live=\"polite\"", simulation);
        Assert.Contains("aria-label=\"Simulation exam pages\"", simulation);
    }

    [Fact]
    public void ProfileDashboardAndSubjectPopup_HaveCompleteAccessibleNames()
    {
        var profile = Read("Views", "User", "Profile.cshtml");
        var dashboard = Read("Views", "Dashboard", "Index.cshtml");
        var popup = Read("Views", "Shared", "_SubjectPopupPartial.cshtml");

        Assert.Contains("for=\"profileName\">Full name", profile);
        Assert.Contains("for=\"profileRole\">Account role", profile);
        Assert.Contains("for=\"subjectSelect\">Select subject for revenue", dashboard);
        Assert.Equal(3, CountOccurrences(dashboard, "role=\"img\" aria-label="));
        Assert.Contains("id=\"subjectPopupLabel\"", popup);
    }

    [Fact]
    public void Views_DoNotUsePositiveTabIndexThatOverridesDomOrder()
    {
        var viewsRoot = Path.Combine(FindProjectRoot(), "ProjectBase", "Views");
        var offenders = Directory
            .EnumerateFiles(viewsRoot, "*.cshtml", SearchOption.AllDirectories)
            .Where(path => System.Text.RegularExpressions.Regex.IsMatch(
                File.ReadAllText(path),
                "tabindex=\"[1-9]"))
            .ToArray();

        Assert.Empty(offenders);
    }

    private static int CountOccurrences(string source, string value) =>
        (source.Length - source.Replace(value, string.Empty).Length) / value.Length;

    private static string Read(params string[] pathParts) =>
        File.ReadAllText(Path.Combine([FindProjectRoot(), "ProjectBase", .. pathParts]));

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
