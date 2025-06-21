namespace PersonalFinanceManager.Application.Services.IServices;

public interface ITransactionTypeService
{
    Task<Result<TransactionTypeDto>> CreateAsync(
        UpsertTransactionTypeDto dto,
        CancellationToken cancellationToken = default
    );
    Task<Result> UpdateAsync(
        Guid id,
        UpsertTransactionTypeDto dto,
        CancellationToken cancellationToken = default
    );
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TransactionTypeDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
    Task<Result<IEnumerable<TransactionTypeDto>>> GetAllAsync(
        CancellationToken cancellationToken = default
    );
}
