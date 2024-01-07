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

    private static readonly IList<string> s_artistAppendixStarters = new List<string>()
    {
        " - ",
        " & ",
        " Vs ",
        " Vs. "
    };

    private static readonly IList<string> s_titleAppendixStarters = new List<string>()
    {
        " - "
    };

    private static IList<char> s_closingBrackets = new List<char> { ')', '}', ']' };

    public static IList<char> ClosingBrackets { get => s_closingBrackets; set => s_closingBrackets = value; }

    public static string? EscapeSpecialChars(this string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return filePath;
        }

        var sb = new StringBuilder(filePath);
        foreach (var kvp in s_charReplacementMap)
        {
            sb.Replace(kvp.Key, kvp.Value);
        }
        return sb.ToString();
    }

    public static string RemovePunctuation(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var result = text.Replace(" and ", "&");
        var punctuation = ".?!,:;––—´‘/…*& #~\\@^|";
        var sb = new StringBuilder(result);
        foreach (var p in punctuation)
        {
            sb.Replace(p.ToString(), string.Empty);
        }
        return sb.ToString();
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
        return RemoveAppendix(text, s_titleAppendixStarters, normalizeMode);
    }

    public static string RemoveArtistAppendix(this string text, NormalizeMode normalizeMode)
    {
        return RemoveAppendix(text, s_artistAppendixStarters, normalizeMode);
    }

    private static string RemoveAppendix(string text, IList<string> endOfContentIndicators, NormalizeMode normalizeMode)
    {
        var result = text;
        foreach (var endOfTitleIndicator in endOfContentIndicators)
        {
            result = RemoveAppendix(result, endOfTitleIndicator, normalizeMode);
        }
        return result;
    }

    public static string RemoveAppendix(this string text, string appendixStarter, NormalizeMode normalizeMode)
    {
        if (normalizeMode == NormalizeMode.Loose || string.IsNullOrEmpty(text))
        {
            return text;
        }

        var transliterated = text.Transliterate();
        var indexOfAppendixStarter = transliterated.IndexOf(appendixStarter, StringComparison.OrdinalIgnoreCase);
        if (indexOfAppendixStarter < 0)
        {
            return transliterated;
        }

        var maxIndexOfNextClosingBracket = -1;
        foreach (var cb in ClosingBrackets)
        {
            var index = transliterated.IndexOf(cb);
            if (index > maxIndexOfNextClosingBracket)
            {
                maxIndexOfNextClosingBracket = index;
            }
        }

        if (maxIndexOfNextClosingBracket == -1)
        {
            return transliterated[..indexOfAppendixStarter].Trim();
        }

        var minIndexOfNextClosingBracket = ClosingBrackets
            .Select(cb => transliterated.IndexOf(cb))
            .Where(i => i != -1)
            .Min();

        var correspondingOpeningBracket = new Dictionary<char, char>
        {
            {')', '('},
            {'}', '{'},
            {']', '['},
        };

        var openingBracket = correspondingOpeningBracket[transliterated[minIndexOfNextClosingBracket]];
        var firstIndexOfPreviousOpeningBracket = transliterated.LastIndexOf(openingBracket, indexOfAppendixStarter);

        var isAppendixStarterBetweenMatchingBrackets = firstIndexOfPreviousOpeningBracket > -1
                                            && firstIndexOfPreviousOpeningBracket < indexOfAppendixStarter
                                            && indexOfAppendixStarter < minIndexOfNextClosingBracket;

        if (isAppendixStarterBetweenMatchingBrackets)
        {
            return new StringBuilder(transliterated[..indexOfAppendixStarter].Trim())
                .Append(transliterated[minIndexOfNextClosingBracket..])
                .ToString();
        }

        return transliterated[..indexOfAppendixStarter].Trim();
    }

    public static string RemoveFeaturingSuffix(this string text)
        => text.Contains("feat.")
            ? text[..text.IndexOf("feat.")].Trim()
            : text;
}

