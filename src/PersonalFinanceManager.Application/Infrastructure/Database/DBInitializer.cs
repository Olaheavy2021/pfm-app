using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using PersonalFinanceManager.Application.Constants;
using PersonalFinanceManager.Application.Models;

namespace PersonalFinanceManager.Application.Infrastructure.Database;

public class DBInitializer()
{
    public async Task SeedData(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();

        try
        {
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            if (!userManager!.Users.Any())
            {
                var user = new ApplicationUser
                {
                    Name = "Admin",
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                if (!await roleManager!.RoleExistsAsync(AppConstants.AdminRole))
                {
                    logger.Information("Admin role is creating");
                    var roleResult = await roleManager.CreateAsync(
                        new IdentityRole(AppConstants.AdminRole)
                    );

                    if (roleResult.Succeeded == false)
                    {
                        var roleErros = roleResult.Errors.Select(e => e.Description);
                        logger.Warning(
                            $"Failed to create admin role. Errors : {string.Join(",", roleErros)}"
                        );

                        return;
                    }
                    logger.Information("Admin role is created");
                }

                // Attempt to create admin user
                var createUserResult = await userManager.CreateAsync(
                    user: user,
                    password: "Admin@123"
                );

                if (createUserResult.Succeeded == false)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description);
                    logger.Warning(
                        $"Failed to create admin user. Errors: {string.Join(", ", errors)}"
                    );
                    return;
                }

                var addUserToRoleResult = await userManager.AddToRoleAsync(
                    user: user,
                    role: AppConstants.AdminRole
                );

                if (addUserToRoleResult.Succeeded == false)
                {
                    var errors = addUserToRoleResult.Errors.Select(e => e.Description);
                    logger.Warning(
                        $"Failed to add admin role to user. Errors : {string.Join(",", errors)}"
                    );
                }
                logger.Information("Admin user is created");
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }
    }
}
