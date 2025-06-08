﻿namespace PersonalFinanceManager.Application.Infrastructure.Settings;

public static class ValidatedOptionsFactory
{
    /// <summary>
    /// A factory for creating validated options based on configuration during startup.
    /// This is required because the default options mechanism requires the options to be
    /// registered with the DI container. This is not useful when you want the validated
    /// options used to configure the DI container during application startup.
    /// </summary>
    public static TOptions Create<TOptions>(IConfiguration configuration)
        where TOptions : IValidatedOptions<TOptions>, new()
    {
        var options = new TOptions();
        var sectionName = options.GetSectionName();
        configuration.GetSection(sectionName).Bind(options);
        var validator = options.GetValidator();
        var result = validator.Validate(options);
        if (!result.IsValid)
        {
            throw new Exception(result.ToString());
        }
        return options;
    }

    public static OptionsBuilder<TOptions> ValidatedOptions<TOptions>(
        this OptionsBuilder<TOptions> builder
    )
        where TOptions : class, IValidatedOptions<TOptions>, new()
    {
        builder.Services.AddSingleton<IValidateOptions<TOptions>>(
            new ValidatedOptions<TOptions>(builder.Name)
        );

        return builder;
    }
}
