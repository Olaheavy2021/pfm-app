namespace PersonalFinanceManager.Application.Services.IServices;

public interface ITransactionCategoryService
{
    Task<Result<TransactionCategoryDto>> CreateAsync(
        UpsertTransactionCategoryDto dto,
        CancellationToken cancellationToken = default
    );
    Task<Result> UpdateAsync(
        Guid id,
        UpsertTransactionCategoryDto dto,
        CancellationToken cancellationToken = default
    );
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TransactionCategoryDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
    Task<Result<IEnumerable<TransactionCategoryDto>>> GetAllAsync(
        CancellationToken cancellationToken = default
    );
}
