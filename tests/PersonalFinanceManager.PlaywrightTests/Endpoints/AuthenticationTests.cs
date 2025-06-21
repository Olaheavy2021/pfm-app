using System.Text.Json;
using AppHost;
using Microsoft.AspNetCore.Authentication.BearerToken;

namespace PersonalFinanceManager.PlaywrightTests.Endpoints;

public class AuthenticationTests(AspireManager aspireManager) : BasePlaywrightTests(aspireManager)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    [IgnoreOnGitHubActionsFact]
    public async Task TestApiGetAuthenticationStatus()
    {
        await SetupAsync();
        await InteractWithPageAsync(
            AppHostConstants.ApiServiceProject,
            async page =>
            {
                var data = new Dictionary<string, object>()
                {
                    { "Email", "admin@gmail.com" },
                    { "Password", "Admin@123" },
                };

                var response = await page.APIRequest.PostAsync(
                    $"{BaseApiUrl}{TestConstants.API_AUTH_LOGIN_ENDPOINT}",
                    new() { DataObject = data }
                );
                var authStatusJson = await response.TextAsync();
                var authStatus = JsonSerializer.Deserialize<AccessTokenResponse>(
                    authStatusJson,
                    _jsonOptions
                );
                Assert.Equal(200, response.Status);
                Assert.NotNull(authStatus);
            }
        );
    }
}
