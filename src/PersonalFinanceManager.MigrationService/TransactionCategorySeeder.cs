using PersonalFinanceManager.Application.Data.Models;

namespace PersonalFinanceManager.MigrationService;

public class TransactionCategorySeeder : ISeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var categories = GetDefaultCategories();
        await dbContext.TransactionCategories.AddRangeAsync(categories, cancellationToken);
    }

    internal static IEnumerable<TransactionCategory> GetDefaultCategories()
    {
        yield return TransactionCategory.Create(
            AppConstants.IncomeTransactionCategory,
            "These are transactions that represent credit and increase total balance per month"
        );
        yield return TransactionCategory.Create(
            AppConstants.ExpenseTransactionCategory,
            "These are transactions that represent debit and reduce total balance per month"
        );
        yield return TransactionCategory.Create(
            AppConstants.DebtTransactionCategory,
            "These are transactions that represent loans, credit cards repayments and borrowings"
        );
        yield return TransactionCategory.Create(
            AppConstants.InvestmentTransactionCategory,
            "These are transactions that represent short and long term investments"
        );
    }
}
