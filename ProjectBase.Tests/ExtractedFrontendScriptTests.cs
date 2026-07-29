namespace ProjectBase.Tests;

public class ExtractedFrontendScriptTests
{
    [Theory]
    [InlineData("BlogIndex.js", ".text(category.title || '')")]
    [InlineData("BlogDetail.js", "document.createTextNode")]
    [InlineData("SimulationExam.js", ".text(exam.examName || '')")]
    [InlineData("SimulationExam.js", "No simulation exams found.")]
    public void ExtractedScripts_KeepSafeRenderingAndEmptyStates(
        string fileName,
        string expectedPattern)
    {
        var script = File.ReadAllText(Path.Combine(
            FindProjectRoot(),
            "ProjectBase",
            "wwwroot",
            "js",
            fileName));

        Assert.Contains(expectedPattern, script);
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
