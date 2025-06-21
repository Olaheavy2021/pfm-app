namespace PersonalFinanceManager.MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger logger
) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity(
            "Migrating database",
            ActivityKind.Client
        );

        try
        {
            logger.Information("Starting migration...");
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var seeders = new ISeeder[] { new TransactionCategorySeeder() };

            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedAllAsync(dbContext, seeders, cancellationToken);
            logger.Information("Migration completed successfully.");
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);

            logger.Error(ex, "Migration failed:");
            logger.Error(ex.Message);
            if (ex.InnerException != null)
            {
                logger.Warning("Inner exception:");
                logger.Error(ex.InnerException.Message);
            }

            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedAllAsync(
        AppDbContext db,
        IEnumerable<ISeeder> seeders,
        CancellationToken ct
    )
    {
        await db
            .Database.CreateExecutionStrategy()
            .ExecuteAsync(
                async innerCt =>
                {
                    await using var tx = await db.Database.BeginTransactionAsync(innerCt);

                    foreach (var seeder in seeders)
                        await seeder.SeedAsync(db, innerCt);

                    await db.SaveChangesAsync(innerCt);
                    await tx.CommitAsync(innerCt);
                },
                ct
            );
    }
}
