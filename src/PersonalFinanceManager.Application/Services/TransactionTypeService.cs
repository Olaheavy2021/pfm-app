namespace PersonalFinanceManager.Application.Services;

internal class TransactionTypeService(
    AppDbContext dbContext,
    IValidator<UpsertTransactionTypeDto> transactionTypeValidator
) : ITransactionTypeService
{
    public async Task<Result<TransactionTypeDto>> CreateAsync(
        UpsertTransactionTypeDto command,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
        //await transactionTypeValidator.ValidateAndThrowAsync(command, cancellationToken);

        //var transactionType = TransactionType.Create(
        //    command.Name,
        //    command.Description,
        //    command.TransactionCategoryId
        //);

        //await dbContext.TransactionTypes.AddAsync(transactionType, cancellationToken);
        //await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<TransactionTypeDto>>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<Result<TransactionTypeDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateAsync(
        Guid id,
        UpsertTransactionTypeDto dto,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
