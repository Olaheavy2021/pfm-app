using System.Net;
using FluentValidation;
using JetBrains.Annotations;

namespace PersonalFinanceManager.API.Infrastructure.Validation;

[UsedImplicitly]
public static class RuleBuilderOptionsExtensions
{
    [UsedImplicitly]
    public static IRuleBuilderOptions<T, TProperty> WithHttpStatus<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> ruleBuilder,
        HttpStatusCode code
    )
    {
        return ruleBuilder.WithState(_ => code);
    }
}
