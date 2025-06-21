namespace PersonalFinanceManager.Application.Data.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}
