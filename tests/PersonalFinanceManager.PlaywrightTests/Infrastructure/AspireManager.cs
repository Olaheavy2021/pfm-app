using Aspire.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PersonalFinanceManager.API;
using PersonalFinanceManager.Test.Infrastructure;
using Xunit.Abstractions;

namespace PersonalFinanceManager.PlaywrightTests.Infrastructure;

/// <summary>
/// Startup and configure the Aspire application for testing.
/// </summary>
public class AspireManager : IAsyncLifetime
{
    internal PlaywrightManager PlaywrightManager { get; } = new();

    internal DistributedApplication? App { get; private set; }

    internal ITestOutputHelper? TestOutput { get; private set; }

    public async Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
        string[]? args = null,
        Action<IDistributedApplicationTestingBuilder>? configureBuilder = null
    )
        where TEntryPoint : class
    {
        if (App is not null)
            return App;

        var builder = await DistributedApplicationTestingBuilder.CreateAsync<TEntryPoint>(
            args: args ?? [],
            configureBuilder: static (options, _) =>
            {
                options.DisableDashboard = false;
            }
        );

        builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";
        builder.RemoveNotNeededResourcesForTesting();
        builder.WithRandomParameterValues();
        builder.WithRandomVolumeNames();
        builder.WithContainersLifetime(ContainerLifetime.Session);

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSimpleConsole();
            logging.AddFakeLogging();
            if (TestOutput is not null)
            {
                logging.AddXUnit(TestOutput);
            }
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddFilter("Aspire", LogLevel.Trace);
            logging.AddFilter(builder.Environment.ApplicationName, LogLevel.Trace);
        });

        configureBuilder?.Invoke(builder);

        App = await builder.BuildAsync();

        await App.StartAsync();

        return App;
    }

    public async Task InitializeAsync()
    {
        await PlaywrightManager.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await PlaywrightManager.DisposeAsync();

        await (App?.DisposeAsync() ?? ValueTask.CompletedTask);
    }
}
