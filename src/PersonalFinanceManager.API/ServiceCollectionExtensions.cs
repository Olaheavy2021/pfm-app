using System.Reflection;
using Carter;
using MediatR;
using PersonalFinanceManager.API.Behaviors;
using PersonalFinanceManager.API.Infrastructure.Behaviors;
using PersonalFinanceManager.Application;
using PersonalFinanceManager.Application.Constants;
using PersonalFinanceManager.Application.Infrastructure.Database;
using PersonalFinanceManager.Application.Infrastructure.OptionsValidation;
using PersonalFinanceManager.Application.Infrastructure.Versioning;

namespace PersonalFinanceManager.API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceCollections(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services
            .AddCustomCors()
            .AddVersioning()
            .AddMediatR([typeof(IApiMarker), typeof(IApplicationMarker)])
            .AddValidators([typeof(IApiMarker), typeof(IApplicationMarker)])
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddProblemDetails()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
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
        DependencyContextAssemblyCatalog assemblyCatalog = new DependencyContextAssemblyCatalog(
            ((IEnumerable<Type>)handlerAssemblyMarkerTypes)
                .Select<Type, Assembly>((Func<Type, Assembly>)(t => t.Assembly))
                .ToArray<Assembly>()
        );
        return services.AddCarter(assemblyCatalog);
    }
}
