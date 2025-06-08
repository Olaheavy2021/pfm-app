namespace PersonalFinanceManager.Application.Infrastructure.MediatR;

public static class ConfigureMediatR
{
    public static IServiceCollection AddMediatR(
        this IServiceCollection services,
        params Type[] validatorAssemblyMarkerTypes
    )
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblies(
                [.. validatorAssemblyMarkerTypes.Select(t => t.Assembly)]
            )
        );
        return services;
    }
}
