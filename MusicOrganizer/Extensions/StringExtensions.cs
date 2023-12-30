using System.Text;
using System.Text.RegularExpressions;
using AnyAscii;
using MusicOrganizer.Models;
using PowerArgs;

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

    private static IList<char> s_closingBrackets = new List<char> { ')', '}', ']' };

    public static IList<char> ClosingBrackets { get => s_closingBrackets; set => s_closingBrackets = value; }

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
    public static string RemoveContentAfterVersus(this string text, NormalizeMode normalizeMode) => RemoveContentAfter(text, " Vs ", normalizeMode);

    public static string RemoveContentAfterVersusDot(this string text, NormalizeMode normalizeMode) => RemoveContentAfter(text, " Vs. ", normalizeMode);

    public static string RemoveContentAfter(this string text, string commentSign, NormalizeMode normalizeMode)
    {
        if (normalizeMode == NormalizeMode.Loose)
        {
            return text;
        }
        var transliterated = text.Transliterate();
        var containsComment = transliterated.Contains(commentSign, StringComparison.InvariantCultureIgnoreCase);
        var maxIndexOfNextClosingBracket = -1;
        foreach (var cb in ClosingBrackets)
        {
            if (transliterated.IndexOf(cb) > maxIndexOfNextClosingBracket)
            {
                maxIndexOfNextClosingBracket = transliterated.IndexOf(cb);
            }
        }
        if (maxIndexOfNextClosingBracket == -1)
        {
            return GetTextBeforeComment(commentSign, transliterated, containsComment);
        }
        var minIndexOfNextClosingBracket = ClosingBrackets
            .Select(cb => transliterated.IndexOf(cb))
            .Where(i => i != -1)
            .Min();
        var correspondingOpeningBracket = new Dictionary<char, char>()
        {
            {')', '('},
            {'}', '{'},
            {']', '['},
        };
        var openingBracket = correspondingOpeningBracket[transliterated[minIndexOfNextClosingBracket]];
        var firstIndexOfPreviousOpeningBracket = transliterated.IndexOf(openingBracket);
        var indexOfCommentSign = transliterated.IndexOf(commentSign);
        var isDashBetweenMatchingBrackets = firstIndexOfPreviousOpeningBracket > -1
            && firstIndexOfPreviousOpeningBracket < indexOfCommentSign
            && indexOfCommentSign < minIndexOfNextClosingBracket;
        if (isDashBetweenMatchingBrackets)
        {
            return new StringBuilder(GetTextBeforeComment(commentSign, transliterated, containsComment))
                .Append(GetTextAfterFirstClosingBracket(transliterated, minIndexOfNextClosingBracket))
                .ToString();
        }
        return GetTextBeforeComment(commentSign, transliterated, containsComment);
    }

    private static string GetTextAfterFirstClosingBracket(string transliterated, int indexOfClosingBracket) => transliterated[indexOfClosingBracket..];

    private static string GetTextBeforeComment(string commentSign, string transliterated, bool containsComment)
    {
        return transliterated[..(containsComment ? transliterated.IndexOf(commentSign, StringComparison.InvariantCultureIgnoreCase)
                                            : transliterated.Length)]
                                            .Trim();
    }

    public static string RemoveFeaturingSuffix(this string text)
        => text.Contains("feat.")
            ? text[..text.IndexOf("feat.")].Trim()
            : text;
}

