namespace PersonalFinanceManager.Application.Settings;

public class AuthOptions : Infrastructure.Settings.IValidatedOptions<AuthOptions>
{
    public string AdminName { get; set; } = default!;
    public string AdminEmail { get; set; } = default!;
    public string AdminPassword { get; set; } = default!;
    public string AdminUserName { get; set; } = default!;

    string Infrastructure.Settings.IValidatedOptions<AuthOptions>.GetSectionName() =>
        GetSectionName();

    public static string GetSectionName() => "Auth";

    public IValidator<AuthOptions> GetValidator() => new Validator();

    private class Validator : AbstractValidator<AuthOptions>
    {
        public Validator()
        {
            RuleFor(x => x.AdminName).NotEmpty();
            RuleFor(x => x.AdminEmail).NotEmpty();
            RuleFor(x => x.AdminPassword).NotEmpty();
            RuleFor(x => x.AdminUserName).NotEmpty();
        }
    }
}
