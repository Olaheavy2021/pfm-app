using PersonalFinanceManager.Application.Data.Models;

namespace PersonalFinanceManager.Application.Infrastructure.Database;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<TokenInfo> TokenInfos { get; set; }

    public DbSet<TransactionCategory> TransactionCategories { get; set; }

    public DbSet<TransactionType> TransactionTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IApplicationMarker).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
