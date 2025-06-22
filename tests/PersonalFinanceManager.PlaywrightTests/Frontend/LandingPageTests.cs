using AppHost;

namespace PersonalFinanceManager.PlaywrightTests.Frontend;

public class LandingPageTests(AspireManager aspireManager) : BasePlaywrightTests(aspireManager)
{
    private readonly string _serviceName = AppHostConstants.FrontendProject;

    [IgnoreOnGitHubActionsFact]
    public async Task TestLandingPageAsync()
    {
        await SetupAsync(_serviceName);
        await InteractWithPageAsync(
            _serviceName,
            async page =>
            {
                await page.GotoAsync("/");

                var title = await page.TitleAsync();
                Assert.Equal("Personal Finance Manager", title);
            }
        );
    }
}
