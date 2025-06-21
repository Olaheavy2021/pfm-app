using System.Text.Json;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Playwright;

namespace PersonalFinanceManager.PlaywrightTests.Helpers;

internal class AuthHelper
{
    public static async Task<string> AuthenticateAsync(
        IPage page,
        string baseApiUrl,
        JsonSerializerOptions jsonOptions
    )
    {
        var loginRequest = new Dictionary<string, object>
        {
            { "Email", "admin@gmail.com" },
            { "Password", "Admin@123" },
        };

        var loginResponse = await page.APIRequest.PostAsync(
            $"{baseApiUrl}{TestConstants.API_AUTH_LOGIN_ENDPOINT}",
            new() { DataObject = loginRequest }
        );

        var authStatusJson = await loginResponse.TextAsync();
        var authStatus = JsonSerializer.Deserialize<AccessTokenResponse>(
            authStatusJson,
            jsonOptions
        );

        if (authStatus?.AccessToken is null)
        {
            throw new InvalidOperationException("Authentication failed: access token is null");
        }

        return authStatus.AccessToken;
    }
}
