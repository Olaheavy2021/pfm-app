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
        await transactionTypeValidator.ValidateAndThrowAsync(command, cancellationToken);

        var transactionType = TransactionType.Create(
            command.Name,
            command.Description,
            command.TransactionCategoryId
        );

        await dbContext.TransactionTypes.AddAsync(transactionType, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var createdType = await GetByIdAsync(transactionType.Id, cancellationToken);

        return Result.Ok(createdType.Value);
    }

    public async Task<Result<IEnumerable<TransactionTypeDto>>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        var mapper = new TransactionTypeMapper();

        var types = await dbContext
            .TransactionTypes.AsNoTracking()
            .Include(t => t.TransactionCategory)
            .Select(type => mapper.ToDto(type))
            .ToListAsync(cancellationToken: cancellationToken);

        return Result.Ok(types.AsEnumerable());
    }

    public async Task<Result<TransactionTypeDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var transactionType = await dbContext
            .TransactionTypes.AsNoTracking()
            .Include(t => t.TransactionCategory)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken: cancellationToken);

        if (transactionType == null)
            return Result.Fail(new Error($"Invalid Transaction Type Id - {id}"));

        var mapper = new TransactionTypeMapper();
        var typeDto = mapper.ToDto(transactionType);

        return Result.Ok(typeDto);
    }

    public async Task<Result> UpdateAsync(
        Guid id,
        UpsertTransactionTypeDto command,
        CancellationToken cancellationToken = default
    )
    {
        var typeToUpdate = await dbContext.TransactionTypes.FindAsync(
            [id],
            cancellationToken: cancellationToken
        );

        if (typeToUpdate is null)
            return Result.Fail(new Error($"Invalid TransactionType Id - {id}"));

        typeToUpdate.Update(
            command.Name,
            command.Description,
            command.Status,
            command.TransactionCategoryId
        );
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
