namespace PersonalFinanceManager.Application.Services;

public class TransactionCategoryService(
    AppDbContext dbContext,
    IValidator<UpsertTransactionCategoryDto> transactionCategoryValidator
) : ITransactionCategoryService
{
    public async Task<Result<TransactionCategoryDto>> CreateAsync(
        UpsertTransactionCategoryDto command,
        CancellationToken cancellationToken = default
    )
    {
        await transactionCategoryValidator.ValidateAndThrowAsync(command, cancellationToken);

        var transactionCategory = TransactionCategory.Create(command.Name, command.Description);

        await dbContext.TransactionCategories.AddAsync(transactionCategory, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var mapper = new TransactionCategoryMapper();
        var dto = mapper.ToDto(transactionCategory);

        return Result.Ok(dto);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var categoryToDelete = await dbContext.TransactionCategories.FindAsync(
            [id],
            cancellationToken: cancellationToken
        );
        if (categoryToDelete != null)
        {
            dbContext.TransactionCategories.Remove(categoryToDelete);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }

        return Result.Fail(new Error($"Invalid Movie Id - {id}"));
    }

    public async Task<Result<IEnumerable<TransactionCategoryDto>>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        var mapper = new TransactionCategoryMapper();

        var category = await dbContext
            .TransactionCategories.AsNoTracking()
            .Select(category => mapper.ToDto(category))
            .ToListAsync(cancellationToken: cancellationToken);

        return Result.Ok(category.AsEnumerable());
    }

    public async Task<Result<TransactionCategoryDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var category = await dbContext
            .TransactionCategories.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken: cancellationToken);

        if (category == null)
            return Result.Fail(new Error($"Invalid Movie Id - {id}"));

        var mapper = new TransactionCategoryMapper();
        var categoryDto = mapper.ToDto(category);

        return Result.Ok(categoryDto);
    }

    public async Task<Result> UpdateAsync(
        Guid id,
        UpsertTransactionCategoryDto command,
        CancellationToken cancellationToken = default
    )
    {
        var categoryToUpdate = await dbContext.TransactionCategories.FindAsync(
            [id],
            cancellationToken: cancellationToken
        );

        if (categoryToUpdate is null)
            return Result.Fail(new Error($"Invalid Movie Id - {id}"));

        categoryToUpdate.Update(command.Name, command.Description, command.Status);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
