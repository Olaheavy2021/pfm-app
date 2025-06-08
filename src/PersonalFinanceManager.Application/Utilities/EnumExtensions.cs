namespace PersonalFinanceManager.Application.Utilities;

public static class EnumExtensions
{
    public static T ToFlags<T>(this IEnumerable<T> values)
        where T : Enum => values.ToFlags<T, T>();

    public static TTarget ToFlags<TSource, TTarget>(this IEnumerable<TSource> values)
        where TSource : Enum
        where TTarget : Enum
    {
        int intResult = values
            .Select(enumVal => Convert.ToInt32(enumVal))
            .Aggregate(0, (current, intVal) => current | intVal);

        return (TTarget)Enum.ToObject(typeof(TTarget), intResult);
    }

    public static TTarget[] ToValues<TSource, TTarget>(this TSource value)
        where TSource : struct, Enum
        where TTarget : struct, Enum =>
        Convert.ToInt32(value) == 0
            ? Array.Empty<TTarget>()
            : value
                .ToString()
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(Enum.Parse<TTarget>)
                .ToArray();

    public static string GetDisplayName<T>(this T value)
        where T : struct, Enum
    {
        var name = Enum.GetName(value)!;
        var displayAttribute = typeof(T).GetField(name)?.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute?.Name ?? name;
    }
}
