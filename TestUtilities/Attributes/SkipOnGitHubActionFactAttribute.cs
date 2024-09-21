namespace TestUtilities.Attributes;

public class SkipOnGitHubFactAttribute : FactAttribute
{
    public SkipOnGitHubFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true")
        {
            Skip = "Test skipped in GitHub Actions.";
        }
    }
}