namespace PersonalFinanceManager.API.Infrastructure.Utils;

public static class StringExtensions
{
    internal static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (!value.Contains('.'))
            return ToCamelCaseSection(value);

        // ReSharper disable once ConvertClosureToMethodGroup
        var parts = value.Split('.').Select(x => ToCamelCaseSection(x));
        return string.Join('.', parts);
    }

    public static string? ConvertToCamelCase(this string? value) =>
        value == null ? value : JsonNamingPolicy.CamelCase.ConvertName(value);

    private static string ToCamelCaseSection(string value)
    {
        if (string.IsNullOrEmpty(value) || !char.IsUpper(value[0]))
        {
            return value;
        }

        var chars = value.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i]))
            {
                break;
            }

            var hasNext = i + 1 < chars.Length;
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                break;
            }

            chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
        }

        return new string(chars);
    }
}
