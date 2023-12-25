namespace MusicOrganizer.Extensions;

public static class Mp3InfoExtensions
{
    public static string? OrderedFirstOrDefault(this IEnumerable<string> str)
        => str.ToOrderedList().FirstOrDefault();
}
