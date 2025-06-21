using AppHost;
using Aspire.Hosting;
using Microsoft.Playwright;
using PersonalFinanceManager.Test.Infrastructure;

namespace PersonalFinanceManager.PlaywrightTests.Infrastructure;

/// <summary>
/// Base class for Playwright tests, providing common functionality and setup for Playwright testing with ASP.NET Core.
/// </summary>
/// <typeparam name="TFixture"></typeparam>
/// <param name="aspireManager"></param>
public abstract class BasePlaywrightTests : IClassFixture<AspireManager>, IAsyncDisposable
{
    protected BasePlaywrightTests(AspireManager aspireManager) =>
        AspireManager = aspireManager ?? throw new ArgumentNullException(nameof(aspireManager));

    AspireManager AspireManager { get; }
    PlaywrightManager PlaywrightManager => AspireManager.PlaywrightManager;
    public string? DashboardUrl { get; private set; }
    public string DashboardLoginToken { get; private set; } = "";
    public string? BaseApiUrl { get; private set; }
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    private IBrowserContext? _context;
    public HttpClient? HttpClient;

    public Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
        string[]? args = null,
        Action<IDistributedApplicationTestingBuilder>? configureBuilder = null
    )
        where TEntryPoint : class =>
        AspireManager.ConfigureAsync<TEntryPoint>(
            args,
            builder =>
            {
                var aspNetCoreUrls = builder.Configuration["ASPNETCORE_URLS"];
                var urls = aspNetCoreUrls is not null ? aspNetCoreUrls.Split(";") : [];

                DashboardUrl = urls.FirstOrDefault();
                DashboardLoginToken = builder.Configuration["AppHost:BrowserToken"] ?? "";

                configureBuilder?.Invoke(builder);
            }
        );

    public async Task InteractWithPageAsync(
        string serviceName,
        Func<IPage, Task> test,
        ViewportSize? size = null
    )
    {
        Uri urlSought;
        var cancellationToken = new CancellationTokenSource(DefaultTimeout).Token;

        // Empty string means the dashboard URL
        if (!string.IsNullOrEmpty(serviceName))
        {
            if (AspireManager.App!.GetEndpoint(serviceName) is null)
            {
                throw new InvalidOperationException(
                    $"Service '{serviceName}' not found in the application endpoints"
                );
            }

            //			urlSought = new Uri(AppHostTestFixture.App.GetEndpoint(serviceName), relativeUrl);
            urlSought = AspireManager.App!.GetEndpoint(serviceName);
        }
        else
        {
            urlSought = new Uri(DashboardUrl!);
        }

        await AspireManager
            .App!.ResourceNotifications.WaitForResourceHealthyAsync(serviceName, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var page = await CreateNewPageAsync(urlSought, size);

        try
        {
            await test(page);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public async Task SetupAsync()
    {
        var app = await ConfigureAsync<Projects.AppHost>();
        HttpClient = app.CreateHttpClient(AppHostConstants.ApiServiceProject);
        BaseApiUrl = HttpClient.BaseAddress?.ToString() ?? string.Empty;
    }

    private async Task<IPage> CreateNewPageAsync(Uri uri, ViewportSize? size = null)
    {
        _context = await PlaywrightManager.Browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                IgnoreHTTPSErrors = true,
                ColorScheme = ColorScheme.Dark,
                ViewportSize = size,
                BaseURL = uri.ToString(),
                RecordVideoDir = "playwright-videos/",
            }
        );

        await _context.Tracing.StartAsync(
            new()
            {
                Title = "ApiMockTraces",
                Screenshots = true,
                Snapshots = true,
                Sources = true,
            }
        );
        return await _context.NewPageAsync();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_context is not null)
        {
            await _context.Tracing.StopAsync(
                new()
                {
                    Path = Path.Combine(
                        Environment.CurrentDirectory,
                        "playwright-traces",
                        "api-mock-traces-with-video.zip"
                    ),
                }
            );
            await _context.DisposeAsync();
        }

        HttpClient?.Dispose();
    }
}
