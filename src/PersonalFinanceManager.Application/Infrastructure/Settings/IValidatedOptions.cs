namespace PersonalFinanceManager.Application.Infrastructure.Settings;

public interface IValidatedOptions<in TOptions>
    where TOptions : new()
{
    string GetSectionName();

    IValidator<TOptions> GetValidator();
}
