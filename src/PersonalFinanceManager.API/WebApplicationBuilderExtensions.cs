namespace PersonalFinanceManager.API;

public static class WebApplicationBuilderExtensions
{
    public static async Task<WebApplication> UseMyAppDefaultsAsync(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseCommonExceptionHandler();
        app.UseCors(AppConstants.CorsPolicy);
        app.NewVersionedApi()
            .MapGroup("/api/auth/v{version:apiVersion}")
            .WithTags("Auth")
            .MapCustomIdentityApi<ApplicationUser>();
        app.NewVersionedApi().MapGroup("/api/v{version:apiVersion}").MapCarter();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle($"{AppConstants.ApplicationName} - Scalar")
                    .WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                    .AddHttpAuthentication("Bearer", options => options.WithToken(""));
            });
        }

        var dbInitializer = app.Services.GetRequiredService<DBInitializer>();
        await dbInitializer.SeedData(app);

        app.UseHttpsRedirection();
        app.UseAuthorization();

        return app;
    }
}
