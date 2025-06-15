namespace PersonalFinanceManager.PlaywrightTests.AppHost;

public class AppHostTests
{
    private static readonly TimeSpan BuildStopTimeout = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan StartStopTimeout = TimeSpan.FromSeconds(120);

    [Fact]
    public async Task AppHostRunsCleanly()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync();

        var resourceNotificationService =
            app.Services.GetRequiredService<ResourceNotificationService>();

        await app.StartAsync().WaitAsync(StartStopTimeout);

        await app.StopAsync().WaitAsync(BuildStopTimeout);
    }
}
