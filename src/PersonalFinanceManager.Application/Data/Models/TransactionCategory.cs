namespace PersonalFinanceManager.Application.Data.Models;

public class TransactionCategory : EntityBase
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public EntityEnum.Status Status { get; private set; } = EntityEnum.Status.Enabled;

    public ICollection<TransactionType> TransactionTypes { get; private set; } =
        new List<TransactionType>();

    public TransactionCategory()
    {
        Name = string.Empty;
        Description = string.Empty;
        Status = EntityEnum.Status.Enabled;
    }

    private TransactionCategory(
        string name,
        string description,
        EntityEnum.Status status = EntityEnum.Status.Enabled
    )
    {
        Name = name;
        Description = description;
        Status = status;
    }

    public static TransactionCategory Create(string name, string description)
    {
        return new TransactionCategory(name, description);
    }

    public void Update(string name, string description, EntityEnum.Status status)
    {
        Name = name;
        Description = description;
        Status = status;

        UpdateLastModified();
    }
}
