var builder = DistributedApplication.CreateBuilder(args);

/*Backing Services */

var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var pfmDb = postgres.AddDatabase("pfmdb");

/*Backing Services*/

/*Projects*/

var migrationService = builder
    .AddProject<Projects.PersonalFinanceManager_MigrationService>("migrationservice")
    .WithReference(pfmDb)
    .WaitFor(pfmDb);

builder
    .AddProject<Projects.PersonalFinanceManager_API>("apiservice")
    .WithReference(pfmDb)
    .WaitFor(pfmDb)
    .WaitFor(migrationService);

/*Projects*/
builder.Build().Run();
