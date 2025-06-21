namespace PersonalFinanceManager.Application.Data.Models;

public class TransactionType : EntityBase
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public EntityEnum.Status Status { get; private set; }
    public Guid TransactionCategoryId { get; private set; }
    public TransactionCategory TransactionCategory { get; private set; } = null!;

    public TransactionType()
    {
        Name = string.Empty;
        Description = string.Empty;
        Status = EntityEnum.Status.Enabled;
        TransactionCategoryId = Guid.Empty;
    }

    private TransactionType(
        string name,
        string description,
        Guid transactionCategoryId,
        EntityEnum.Status status = EntityEnum.Status.Enabled
    )
    {
        Name = name;
        Description = description;
        Status = status;
        TransactionCategoryId = transactionCategoryId;
    }

    public static TransactionType Create(
        string name,
        string description,
        Guid transactionCategoryId
    )
    {
        return new TransactionType(name, description, transactionCategoryId);
    }

    public void Update(
        string name,
        string description,
        EntityEnum.Status status,
        Guid transactionCategoryId
    )
    {
        Name = name;
        Description = description;
        Status = status;
        TransactionCategoryId = transactionCategoryId;

        UpdateLastModified();
    }
}
