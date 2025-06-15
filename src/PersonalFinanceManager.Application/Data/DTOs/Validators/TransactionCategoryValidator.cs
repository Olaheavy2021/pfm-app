namespace PersonalFinanceManager.Application.Data.DTOs.Validators;

public class TransactionCategoryValidator : AbstractValidator<UpsertTransactionCategoryDto>
{
    public TransactionCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(30)
            .WithMessage("Name must not exceed 30 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description must not exceed 200 characters.");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Status must be a valid status enum value.");
    }
}
