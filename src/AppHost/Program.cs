var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PersonalFinanceManager_API>("personalfinancemanager-api");

builder.Build().Run();
