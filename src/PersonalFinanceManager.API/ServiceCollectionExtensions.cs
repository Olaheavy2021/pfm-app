namespace PersonalFinanceManager.API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceCollections(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services
            .AddApplicationOptions(config)
            .AddCustomCors()
            .AddVersioning()
            .AddMediatR([typeof(IApiMarker), typeof(IApplicationMarker)])
            .AddValidators([typeof(IApiMarker), typeof(IApplicationMarker)])
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddProblemDetails()
            .AddEndpointsApiExplorer()
            .AddCarterModules(typeof(IApiMarker));

        services.AddDatabase(config[AppConstants.DbConnectionString]!);
        services.AddIdentity();

        return services;
    }

    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddPolicy(
                name: AppConstants.CorsPolicy,
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            )
        );
        return services;
    }

    public static IServiceCollection AddCarterModules(
        this IServiceCollection services,
        params Type[] handlerAssemblyMarkerTypes
    )
    {
        DependencyContextAssemblyCatalog assemblyCatalog = new(
            [.. handlerAssemblyMarkerTypes.Select(t => t.Assembly)]
        );
        return services.AddCarter(assemblyCatalog);
    }

    public static IServiceCollection AddApplicationOptions(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services
            .AddOptions<AuthOptions>()
            .Bind(config.GetSection(AuthOptions.GetSectionName()))
            .ValidatedOptions();

        return services;
    }
}
