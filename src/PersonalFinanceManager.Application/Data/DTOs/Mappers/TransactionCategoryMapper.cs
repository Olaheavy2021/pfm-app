namespace PersonalFinanceManager.Application.Data.Mappers;

[Mapper]
public partial class TransactionCategoryMapper
{
    public partial TransactionCategoryDto ToDto(TransactionCategory transactionCategory);
}
