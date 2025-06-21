namespace PersonalFinanceManager.Application.Data.Mappers;

[Mapper]
public partial class TransactionTypeMapper
{
    public partial TransactionTypeDto ToDto(TransactionType transactionType);
}
