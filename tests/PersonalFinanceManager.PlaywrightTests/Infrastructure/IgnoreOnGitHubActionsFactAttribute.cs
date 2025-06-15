namespace PersonalFinanceManager.PlaywrightTests.Infrastructure;

public sealed class IgnoreOnGitHubActionsFactAttribute : FactAttribute
{
    public IgnoreOnGitHubActionsFactAttribute()
    {
        if (IsRunningOnGitHub())
        {
            Skip = "Ignored on GitHub Actions Runner";
        }
    }

    public static bool IsRunningOnGitHub()
    {
        //check if it is equals to true also
        return Environment.GetEnvironmentVariable("CI") != null;
    }
}

public sealed class IgnoreOnGitHubActionsTheoryAttribute : TheoryAttribute
{
    public IgnoreOnGitHubActionsTheoryAttribute()
    {
        if (!IsRunningOnGitHub())
        {
            return;
        }

        Skip = "Ignored on GitHub Actions";
    }

    public static bool IsRunningOnGitHub()
    {
        return Environment.GetEnvironmentVariable("CI") != null;
    }
}
