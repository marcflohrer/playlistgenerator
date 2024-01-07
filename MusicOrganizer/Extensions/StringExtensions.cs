using System.Text;
using System.Text.RegularExpressions;
using AnyAscii;
using MusicOrganizer.Models;
using PowerArgs;

namespace MusicOrganizer.Extensions;

public static partial class StringExtensions
{
    private static readonly Dictionary<string, string> s_charReplacementMap = new()
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

    private static readonly IList<string> s_beginOfArtistAppendixIndicators = new List<string>()
    {
        " - ",
        " & ",
        " Vs ",
        " Vs. "
    };

    private static readonly IList<string> s_beginOfTitleAppendixIndicators = new List<string>()
    {
        " - "
    };

    private static IList<char> s_closingBrackets = new List<char> { ')', '}', ']' };

    public static IList<char> ClosingBrackets { get => s_closingBrackets; set => s_closingBrackets = value; }

    public static string? EscapeSpecialChars(this string? filePath)
    {
        if (filePath == null)
        {
            return null;
        }
        foreach (var kvp in s_charReplacementMap)
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
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        return Regex.Replace(text, @"\{.*?\}|\[.*?\]|\(.*?\)", "");
    }

    public static string ReplaceSpotifyTagErrors(this string text, List<MusicBrainzTagMap> tagMaps)
    {
        var tagErrorsFixed = text;
        // Replace longer tags first in case one tag is a substring of another.
        Sort(tagMaps);
        foreach (var tagFix in tagMaps)
        {
            if (tagErrorsFixed.Contains(tagFix.SpotifyTag, StringComparison.InvariantCultureIgnoreCase)
                                     && !tagErrorsFixed.Equals(tagFix.MusicBrainzTag, StringComparison.InvariantCultureIgnoreCase))
            {
                tagErrorsFixed = tagErrorsFixed.Replace(tagFix.SpotifyTag, tagFix.MusicBrainzTag, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        return tagErrorsFixed;
    }

    private static void Sort(List<MusicBrainzTagMap> tagMaps)
    {
        tagMaps.Sort((MusicBrainzTagMap tm1, MusicBrainzTagMap tm2) => tm2.SpotifyTag.Length.CompareTo(tm1.SpotifyTag.Length));
    }

    public static string RemoveTitleAppendix(this string text, NormalizeMode normalizeMode)
    {
        return RemoveAppendix(text, s_beginOfTitleAppendixIndicators, normalizeMode);
    }

    public static string RemoveArtistAppendix(this string text, NormalizeMode normalizeMode)
    {
        return RemoveAppendix(text, s_beginOfArtistAppendixIndicators, normalizeMode);
    }

    private static string RemoveAppendix(string text, IList<string> endOfContentIndicators, NormalizeMode normalizeMode)
    {
        var result = text;
        foreach (var endOfTitleIndicator in endOfContentIndicators)
        {
            result = RemoveContentAfter(result, endOfTitleIndicator, normalizeMode);
        }
        return result;
    }

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

