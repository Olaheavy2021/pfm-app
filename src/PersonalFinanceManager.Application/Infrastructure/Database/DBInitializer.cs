﻿using PersonalFinanceManager.Application.Settings;

namespace PersonalFinanceManager.Application.Infrastructure.Database;

public class DBInitializer()
{
    public async Task SeedData(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
        var authOptions = scope.ServiceProvider.GetRequiredService<IOptions<AuthOptions>>().Value;

        try
        {
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            if (!userManager!.Users.Any())
            {
                var user = new ApplicationUser
                {
                    Name = authOptions.AdminName,
                    UserName = authOptions.AdminUserName,
                    Email = authOptions.AdminEmail,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                if (!await roleManager!.RoleExistsAsync(AppConstants.AdminRole))
                {
                    logger.Information("Admin role is creating");

                    var adminRoleResult = await roleManager.CreateAsync(
                        new IdentityRole(AppConstants.AdminRole)
                    );

                    if (!adminRoleResult.Succeeded)
                    {
                        var roleErros = adminRoleResult.Errors.Select(e => e.Description);
                        logger.Warning(
                            $"Failed to create admin role. Errors : {string.Join(",", roleErros)}"
                        );

                        return;
                    }
                    logger.Information("Admin role is created");

                    var userRoleResult = await roleManager.CreateAsync(
                        new IdentityRole(AppConstants.UserRole)
                    );

                    if (!userRoleResult.Succeeded)
                    {
                        var roleErros = userRoleResult.Errors.Select(e => e.Description);
                        logger.Warning(
                            $"Failed to create user role. Errors : {string.Join(",", roleErros)}"
                        );

                        return;
                    }
                    logger.Information("User role is created");
                }

                // Attempt to create admin user
                var createUserResult = await userManager.CreateAsync(
                    user: user,
                    password: authOptions.AdminPassword
                );

                if (!createUserResult.Succeeded)
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

                if (!addUserToRoleResult.Succeeded)
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
