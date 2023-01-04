using System;
namespace MusicOrganizer.Extensions;

public static class StringArrayExtensions
{
    public static string ArrayToString(this string[] array) => string.Join("", array);
}

