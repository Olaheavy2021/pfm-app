using System.Text.Json;
using AppHost;

namespace PersonalFinanceManager.PlaywrightTests.Endpoints;

public class WeatherForecastTests(AspireManager aspireManager) : BasePlaywrightTests(aspireManager)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    [IgnoreOnGitHubActionsFact]
    public async Task TestApiGetWeatherForecasts()
    {
        await SetupAsync();

        await InteractWithPageAsync(
            AppHostConstants.ApiServiceProject,
            async page =>
            {
                var response = await page.APIRequest.GetAsync(
                    $"{BaseApiUrl}{TestConstants.API_WEATHER_FORECAST_ENDPOINT}"
                );
                var weatherForecastsJson = await response.TextAsync();
                var weatherForecasts = JsonSerializer.Deserialize<List<WeatherForecastResponse>>(
                    weatherForecastsJson,
                    _jsonOptions
                );
                Assert.Equal(200, response.Status);
                Assert.NotNull(weatherForecasts);
                Assert.True(weatherForecasts.Count > 0);
            }
        );
    }

    public record WeatherForecastResponse(DateOnly Date, int TemperatureC, string? Summary);
}
