using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentValidation.Internal;
using PersonalFinanceManager.API.Infrastructure.Utils;
using PersonalFinanceManager.Application.Infrastructure.OptionsValidation;

namespace PersonalFinanceManager.Application.Infrastructure.OptionsValidation;

public static class ValidatorNameResolver
{
    public static string? ResolvePropertyName(
        Type _,
        MemberInfo? memberInfo,
        LambdaExpression? expression
    )
    {
        if (expression == null)
            return memberInfo?.Name;

        var chain = PropertyChain.FromExpression(expression);
        var propertyName = chain.Count > 0 ? chain.ToString() : memberInfo?.Name;
        return propertyName?.ToCamelCase();
    }

    public static string? ResolveDisplayName(Type _, MemberInfo? memberInfo, LambdaExpression __) =>
        memberInfo?.Name.SplitPascalCase();

    private static string SplitPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var retVal = new StringBuilder(input.Length + 5);

        for (var i = 0; i < input.Length; ++i)
        {
            var currentChar = input[i];
            if (IsUpperCaseWithMixedNeighbors(input, i, currentChar))
            {
                retVal.Append(' ');
            }

            if (ShouldAppendChar(input, i, currentChar))
            {
                retVal.Append(currentChar);
            }
        }

        return retVal.ToString().Trim();
    }

    private static bool IsUpperCaseWithMixedNeighbors(string input, int index, char currentChar)
    {
        return char.IsUpper(currentChar)
            && (
                index > 1 && !char.IsUpper(input[index - 1])
                || index + 1 < input.Length && !char.IsUpper(input[index + 1])
            );
    }

    private static bool ShouldAppendChar(string input, int index, char currentChar)
    {
        return currentChar != '.' || index + 1 == input.Length || !char.IsUpper(input[index + 1]);
    }
}
