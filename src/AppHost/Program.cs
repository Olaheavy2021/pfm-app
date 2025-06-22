var builder = DistributedApplication.CreateBuilder(args);

/*Backing Services */

var postgres = builder
    .AddPostgres(AppHostConstants.PostgresServerBackingService)
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var pfmDb = postgres.AddDatabase(AppHostConstants.PfmDbBackingService);

if (builder.ExecutionContext.IsRunMode)
{
    postgres.WithDataVolume(AppHostConstants.PfmDbVolumeBackingService);
}

/*Backing Services*/

/*Projects*/

var migrationService = builder
    .AddProject<Projects.PersonalFinanceManager_MigrationService>(
        AppHostConstants.MigrationServiceProject
    )
    .WithReference(pfmDb)
    .WaitFor(pfmDb);

var apiService = builder
    .AddProject<Projects.PersonalFinanceManager_API>(AppHostConstants.ApiServiceProject)
    .WithReference(pfmDb)
    .WaitFor(pfmDb)
    .WaitFor(migrationService);

builder
    .AddNpmApp(AppHostConstants.FrontendProject, "../PersonalFinanceManager.Frontend")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithHttpEndpoint(port: 3039, targetPort: 3039, isProxied: false)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

/*Projects*/

builder.Build().Run();
