namespace PersonalFinanceManager.Application.Data.DTOs.Validators;

public class TransactionTypeValidator : AbstractValidator<UpsertTransactionTypeDto>
{
    private readonly AppDbContext _dbContext;

    public TransactionTypeValidator(AppDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(30)
            .WithMessage("Name must not exceed 30 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description must not exceed 200 characters.");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Status must be a valid status enum value.");

        RuleFor(x => x.TransactionCategoryId)
            .NotEmpty()
            .WithMessage("Transaction Category is required.")
            .MustAsync(TransactionCategoryExists)
            .WithMessage("Transaction Category does not exist.");
    }

    private async Task<bool> TransactionCategoryExists(
        Guid transactionCategoryId,
        CancellationToken token
    )
    {
        return await _dbContext.TransactionCategories.AnyAsync(
            x => x.Id == transactionCategoryId && x.Status == EntityEnum.Status.Enabled,
            cancellationToken: token
        );
    }
}
