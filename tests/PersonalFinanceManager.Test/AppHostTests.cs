﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using AppHost;
using PersonalFinanceManager.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PersonalFinanceManager.Test;

public class AppHostTests(ITestOutputHelper testOutput)
{
    private static readonly TimeSpan BuildStopTimeout = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan StartStopTimeout = TimeSpan.FromSeconds(120);

    [Theory]
    [MemberData(nameof(AppHostAssemblies))]
    public async Task AppHostRunsCleanly(string appHostPath)
    {
        var appHost = await DistributedApplicationTestFactory.CreateAsync(appHostPath, testOutput);
        await using var app = await appHost.BuildAsync().WaitAsync(BuildStopTimeout);

        await app.StartAsync().WaitAsync(StartStopTimeout);
        await app.WaitForResourcesAsync().WaitAsync(StartStopTimeout);

        //app.EnsureNoErrorsLogged();

        await app.StopAsync().WaitAsync(BuildStopTimeout);
    }

    [Theory]
    [MemberData(nameof(TestEndpoints))]
    public async Task TestEndpointsReturnOk(TestEndpoints testEndpoints)
    {
        var appHostName = testEndpoints.AppHost!;
        var resourceEndpoints = testEndpoints.ResourceEndpoints!;

        var appHostPath = $"{appHostName}.dll";
        var appHost = await DistributedApplicationTestFactory.CreateAsync(appHostPath, testOutput);
        var projects = appHost.Resources.OfType<ProjectResource>();
        await using var app = await appHost.BuildAsync().WaitAsync(BuildStopTimeout);

        await app.StartAsync().WaitAsync(StartStopTimeout);
        await app.WaitForResourcesAsync().WaitAsync(StartStopTimeout);

        if (testEndpoints.WaitForResources?.Count > 0)
        {
            // Wait until each resource transitions to the required state
            var timeout = TimeSpan.FromMinutes(5);
            foreach (var (ResourceName, TargetState) in testEndpoints.WaitForResources)
            {
                await app.WaitForResource(ResourceName, TargetState).WaitAsync(timeout);
            }
        }

        foreach (var resource in resourceEndpoints.Keys)
        {
            var endpoints = resourceEndpoints[resource];

            if (endpoints.Count == 0)
            {
                // No test endpoints so ignore this resource
                continue;
            }

            HttpResponseMessage? response = null;

            using var client = app.CreateHttpClient(
                resource,
                null,
                clientBuilder =>
                    clientBuilder
                        .ConfigureHttpClient(client => client.Timeout = Timeout.InfiniteTimeSpan)
                        .AddStandardResilienceHandler(resilience =>
                        {
                            resilience.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(120);
                            resilience.AttemptTimeout.Timeout = TimeSpan.FromSeconds(60);
                            resilience.Retry.MaxRetryAttempts = 30;
                            resilience.CircuitBreaker.SamplingDuration =
                                resilience.AttemptTimeout.Timeout * 2;
                        })
            );

            await app
                .ResourceNotifications.WaitForResourceAsync(
                    AppHostConstants.ApiServiceProject,
                    KnownResourceStates.Running
                )
                .WaitAsync(TimeSpan.FromSeconds(30));

            foreach (var path in endpoints)
            {
                if (
                    string.Equals(
                        "/ApplyDatabaseMigrations",
                        path,
                        StringComparison.OrdinalIgnoreCase
                    )
                    && projects.FirstOrDefault(p =>
                        string.Equals(p.Name, resource, StringComparison.OrdinalIgnoreCase)
                    )
                        is { } project
                )
                {
                    await app.TryApplyEfMigrationsAsync(project);
                    continue;
                }

                testOutput.WriteLine(
                    $"Calling endpoint '{client.BaseAddress}{path.TrimStart('/')} for resource '{resource}' in app '{Path.GetFileNameWithoutExtension(appHostPath)}'"
                );
                try
                {
                    response = await client.GetAsync(path);
                }
                catch (Exception e)
                {
                    throw new XunitException(
                        $"Failed calling endpoint '{client.BaseAddress}{path.TrimStart('/')} for resource '{resource}' in app '{Path.GetFileNameWithoutExtension(appHostPath)}'",
                        e
                    );
                }

                Assert.True(
                    HttpStatusCode.OK == response.StatusCode,
                    $"Endpoint '{client.BaseAddress}{path.TrimStart('/')}' for resource '{resource}' in app '{Path.GetFileNameWithoutExtension(appHostPath)}' returned status code {response.StatusCode}"
                );
            }
        }

        //app.EnsureNoErrorsLogged();

        await app.StopAsync().WaitAsync(BuildStopTimeout);
    }

    public static TheoryData<string> AppHostAssemblies()
    {
        var appHostAssemblies = GetSamplesAppHostAssemblyPaths();
        var theoryData = new TheoryData<string, bool>();
        return
        [
            .. appHostAssemblies.Select(p => Path.GetRelativePath(AppContext.BaseDirectory, p)),
        ];
    }

    public static TheoryData<TestEndpoints> TestEndpoints()
    {
        return new(
            [
                new TestEndpoints(
                    "AppHost",
                    new() { { "apiservice", ["/alive", "/health", "api/v1/weatherForecasts"] } }
                ),
            ]
        );
    }

    private static IEnumerable<string> GetSamplesAppHostAssemblyPaths()
    {
        // All the AppHost projects are referenced by this project so we can find them by looking for all their assemblies in the base directory
        return Directory
            .GetFiles(AppContext.BaseDirectory, "AppHost.dll")
            .Where(fileName =>
                !fileName.EndsWith("Aspire.Hosting.AppHost.dll", StringComparison.OrdinalIgnoreCase)
            );
    }
}

public class TestEndpoints : IXunitSerializable
{
    // Required for deserialization
    public TestEndpoints() { }

    public TestEndpoints(string appHost, Dictionary<string, List<string>> resourceEndpoints)
    {
        AppHost = appHost;
        ResourceEndpoints = resourceEndpoints;
    }

    public string? AppHost { get; set; }

    public List<ResourceWait>? WaitForResources { get; set; }

    public Dictionary<string, List<string>>? ResourceEndpoints { get; set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
        AppHost = info.GetValue<string>(nameof(AppHost));
        WaitForResources = JsonSerializer.Deserialize<List<ResourceWait>>(
            info.GetValue<string>(nameof(WaitForResources))
        );
        ResourceEndpoints = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
            info.GetValue<string>(nameof(ResourceEndpoints))
        );
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(AppHost), AppHost);
        info.AddValue(nameof(WaitForResources), JsonSerializer.Serialize(WaitForResources));
        info.AddValue(nameof(ResourceEndpoints), JsonSerializer.Serialize(ResourceEndpoints));
    }

    public override string? ToString() => $"{AppHost} ({ResourceEndpoints?.Count ?? 0} resources)";

    public class ResourceWait(string resourceName, string targetState)
    {
        public string ResourceName { get; } = resourceName;

        public string TargetState { get; } = targetState;

        public void Deconstruct(out string resourceName, out string targetState)
        {
            resourceName = ResourceName;
            targetState = TargetState;
        }
    }
}
