using System;
using System.Text.RegularExpressions;

namespace MusicOrganizer.Extensions;

public static class StringExtensions
{
    public static Dictionary<string, string> CharReplacementMap = new Dictionary<string, string>()
    {
        {" ", @"\ "},
        { ",", @"\,"},
        {"'", @"\'"},
        {"`", @"\`"},
        {"´", @"\´"},
        {"(", @"\("},
        {")", @"\)"},
        {"[", @"\["},
        {"]", @"\]"},
        {"{", @"\{"},
        {"}", @"\}"},
        {"\"", @"\"""},
        {"&", @"\&"},
        {";", @"\;"},
        {"<", @"\<"},
        {">", @"\>"}
    };

    public static string? EscapeSpecialChars(this string? filePath)
    {
        if (filePath == null)
        {
            return null;
        }
        foreach (var kvp in CharReplacementMap)
        {
            filePath = filePath?.Replace(kvp.Key, kvp.Value);
        }
        return filePath;
    }

    public static string RemovePunctuation(this string text)
    {
        var result = text.Replace(" and ", "&");
        var punctuation = ".?!,:;––—'´‘/…*& #~\\@^|";
        foreach (var p in punctuation.ToList())
        {
            result = result.Replace(p.ToString(), string.Empty);
        }
        return result;
    }

    public static string RemoveContentInBrackets(this string text)
    {
        var noRoundBracketsContent = Regex.Replace(text, "(.*)(\\(.*\\))(.*)", "$1$3");
        var noSquareBracketsContent = Regex.Replace(noRoundBracketsContent, "(.*)(\\[.*\\])(.*)", "$1$3");
        return Regex.Replace(noSquareBracketsContent, "(.*)(\\{.*\\})(.*)", "$1$3");
    }
}

