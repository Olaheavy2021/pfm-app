namespace PersonalFinanceManager.MigrationService;

public interface ISeeder
{
    Task SeedAsync(AppDbContext db, CancellationToken ct);
}
