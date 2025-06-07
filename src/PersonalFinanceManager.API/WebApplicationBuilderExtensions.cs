using Carter;
using PersonalFinanceManager.Application.Constants;
using PersonalFinanceManager.Application.Infrastructure.Database;

namespace PersonalFinanceManager.API;

public static class WebApplicationBuilderExtensions
{
    public static async Task<WebApplication> UseMyAppDefaultsAsync(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseCommonExceptionHandler();
        app.UseCors(AppConstants.CorsPolicy);
        app.NewVersionedApi().MapGroup("/api/v{version:apiVersion}").MapCarter();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
                options.SwaggerEndpoint("/openapi/v1.json", AppConstants.ApplicationName)
            );
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        var dbInitializer = app.Services.GetRequiredService<DBInitializer>();
        await dbInitializer.SeedData(app);

        return app;
    }
}
