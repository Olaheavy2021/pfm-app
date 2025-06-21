using System.Text.Json;
using AppHost;
using Microsoft.AspNetCore.Authentication.BearerToken;
using PersonalFinanceManager.PlaywrightTests.Helpers;

namespace PersonalFinanceManager.PlaywrightTests.Endpoints;

public class AuthenticationTests(AspireManager aspireManager) : BasePlaywrightTests(aspireManager)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    [IgnoreOnGitHubActionsFact]
    public async Task TestApiLogin()
    {
        await SetupAsync();
        await InteractWithPageAsync(
            AppHostConstants.ApiServiceProject,
            async page =>
            {
                var token = await AuthHelper.AuthenticateAsync(page, BaseApiUrl!, _jsonOptions);
                Assert.NotNull(token);
            }
        );
    }
}
