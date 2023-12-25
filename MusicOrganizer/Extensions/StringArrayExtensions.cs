namespace MusicOrganizer.Extensions;

public static class StringArrayExtensions
{
    public static string ArrayToString(this string[] array) => string.Join("", array);

    public static IOrderedEnumerable<string> ToOrderedList(this IEnumerable<string> strings) => strings.OrderBy(s => s);
}

