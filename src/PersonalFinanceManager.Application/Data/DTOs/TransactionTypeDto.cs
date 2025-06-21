using Humanizer;

namespace PersonalFinanceManager.Application.Data.DTOs;

public record UpsertTransactionTypeDto(
    string Name,
    string Description,
    Guid TransactionCategoryId,
    EntityEnum.Status Status = EntityEnum.Status.Enabled
);

public record TransactionTypeDto(
    Guid Id,
    string Name,
    string Description,
    EntityEnum.Status Status,
    DateTimeOffset Created,
    DateTimeOffset LastModified,
    TransactionCategoryDto TransactionCategory,
    Guid TransactionCategoryId
)
{
    public string StatusText => Status.ToString().Humanize();
    public string CreatedAgo => Created.Humanize();
    public string LastModifiedAgo => LastModified.Humanize();
}
