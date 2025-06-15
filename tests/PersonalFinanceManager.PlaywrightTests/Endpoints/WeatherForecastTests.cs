using System.Net.Http.Json;

namespace PersonalFinanceManager.PlaywrightTests.Endpoints;

public class WeatherForecastTests : BasePlaywrightTests
{
    public WeatherForecastTests(AspireManager aspireManager)
        : base(aspireManager) { }

    [Fact]
    public async Task TestApiGetWeatherForecast()
    {
        await SetupAsync();
        var response = await HttpClient!.GetAsync("/api/v1/weatherForecasts");
        response.EnsureSuccessStatusCode();
        var weatherForecasts =
            await response.Content.ReadFromJsonAsync<WeatherForecastResponse[]>();
        Assert.NotNull(weatherForecasts);
        Assert.True(weatherForecasts.Length > 0);
    }

    public record WeatherForecastResponse(DateOnly Date, int TemperatureC, string? Summary);
}
