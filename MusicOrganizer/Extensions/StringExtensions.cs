using System.Text.RegularExpressions;
using AnyAscii;
using MusicOrganizer.Models;

namespace MusicOrganizer.Extensions;

public static partial class StringExtensions
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
        var punctuation = ".?!,:;––—´‘/…*& #~\\@^|";
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

    public static string ReplaceRockNRoll(this string text) => text.Replace("Rock \u0026 Roll", "rockroll", StringComparison.InvariantCultureIgnoreCase);

    public static string ReplaceSpotifyTagErrors(this string text, IList<MusicBrainzTagMap> tagMaps)
    {
        var tagErrorsFixed = text;
        foreach (var tagFix in from tagFix in tagMaps
                               where tagErrorsFixed.Contains(tagFix.SpotifyTag, StringComparison.InvariantCultureIgnoreCase)
                                     && !tagErrorsFixed.Contains(tagFix.MusicBrainzTag, StringComparison.InvariantCultureIgnoreCase)
                               select tagFix)
        {
            tagErrorsFixed = tagErrorsFixed.Replace(tagFix.SpotifyTag, tagFix.MusicBrainzTag, StringComparison.InvariantCultureIgnoreCase);
        }

        return tagErrorsFixed;
    }

    public static string RemoveContentAfterDash(this string text, NormalizeMode normalizeMode) => RemoveContentAfter(text, " - ", normalizeMode);

    public static string RemoveContentAfterAmpersand(this string text, NormalizeMode normalizeMode) => RemoveContentAfter(text, " & ", normalizeMode);

    public static string RemoveContentAfter(this string text, string commentSign, NormalizeMode normalizeMode)
    {
        if (normalizeMode != NormalizeMode.Strict)
        {
            return text;
        }
        var transliterated = text.Transliterate();
        var containsDash = transliterated.Contains(commentSign, StringComparison.CurrentCulture);
        return transliterated[..(containsDash ? transliterated.IndexOf(commentSign)
                                    : transliterated.Length)]
                                    .Trim();
    }

    public static string RemoveFeaturingSuffix(this string text)
        => text.Contains("feat.")
            ? text[..text.IndexOf("feat.")].Trim()
            : text;
}

