using PersonalFinanceManager.Application.Data.Models;

namespace PersonalFinanceManager.MigrationService;

public class TransactionTypeSeeder : ISeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var hasData = await dbContext.TransactionTypes.AnyAsync(cancellationToken);
        if (hasData)
            return;

        var categories = GetDefaultCategories(dbContext);
        await dbContext.TransactionTypes.AddRangeAsync(categories, cancellationToken);
    }

    private static IEnumerable<TransactionType> GetDefaultCategories(AppDbContext dbContext)
    {
        var categoryMap = dbContext
            .TransactionCategories.AsNoTracking()
            .ToDictionary(c => c.Name, c => c.Id);

        if (categoryMap.TryGetValue(AppConstants.IncomeTransactionCategory, out var incomeId))
        {
            yield return TransactionType.Create("Salary", "Monthly salary", incomeId);
            yield return TransactionType.Create("Bonus", "Yearly bonus", incomeId);
        }

        if (categoryMap.TryGetValue(AppConstants.ExpenseTransactionCategory, out var expenseId))
        {
            yield return TransactionType.Create(
                "Groceries",
                "Monthly groceries expenses",
                expenseId
            );
            yield return TransactionType.Create(
                "Child Expense",
                "Monthly Expenses on the children",
                expenseId
            );
            yield return TransactionType.Create("Water Bill", "Monthly Bill for Water", expenseId);
            yield return TransactionType.Create(
                "Electricity Bill",
                "Monthly Bill for Electricity",
                expenseId
            );
            yield return TransactionType.Create("Council Tax", "Monthly Bill for Tax", expenseId);
            yield return TransactionType.Create("Rent", "Monthly rent payment", expenseId);
        }

        if (categoryMap.TryGetValue(AppConstants.DebtTransactionCategory, out var debtId))
        {
            yield return TransactionType.Create(
                "Credit Card Payment",
                "Monthly credit card payment",
                debtId
            );
            yield return TransactionType.Create("Loan Repayment", "Monthly loan repayment", debtId);
        }

        if (
            categoryMap.TryGetValue(
                AppConstants.InvestmentTransactionCategory,
                out var investmentId
            )
        )
        {
            yield return TransactionType.Create(
                "Stocks Purchase",
                "Purchase of stocks for investment",
                investmentId
            );
            yield return TransactionType.Create(
                "Mutual Funds Investment",
                "Investment in mutual funds",
                investmentId
            );
        }
    }
}
