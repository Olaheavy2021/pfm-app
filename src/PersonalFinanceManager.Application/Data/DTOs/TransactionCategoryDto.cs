using PersonalFinanceManager.Application.Data.Models;

namespace PersonalFinanceManager.Application.Data.DTOs;

public record UpsertTransactionCategoryDto(
    string Name,
    string Description,
    EntityEnum.Status Status = EntityEnum.Status.Enabled
);

public record TransactionCategoryDto(
    Guid Id,
    string Name,
    string Description,
    EntityEnum.Status Status,
    DateTimeOffset Created,
    DateTimeOffset LastModified
);
