using AppHost;
using Xunit.Abstractions;

namespace PersonalFinanceManager.PlaywrightTests.Endpoints;

public class WeatherForecastTests()
{
    [Fact]
    public async Task TestApiGetWeatherForecast()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync();

        var resourceNotificationService =
            app.Services.GetRequiredService<ResourceNotificationService>();

        await app.StartAsync();

        // Act
        var httpClient = app.CreateHttpClient(AppHostConstants.ApiServiceProject);

        await resourceNotificationService
            .WaitForResourceAsync(AppHostConstants.ApiServiceProject, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30));

        var response = await httpClient.GetAsync("/api/v1/weatherForecasts");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
