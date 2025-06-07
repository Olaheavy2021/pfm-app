using AppHost;

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

builder
    .AddProject<Projects.PersonalFinanceManager_API>(AppHostConstants.ApiServiceProject)
    .WithReference(pfmDb)
    .WaitFor(pfmDb)
    .WaitFor(migrationService);

/*Projects*/
builder.Build().Run();
