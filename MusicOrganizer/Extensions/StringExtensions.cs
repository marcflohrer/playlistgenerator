using System.Text;
using System.Text.RegularExpressions;
using AnyAscii;
using MusicOrganizer.Models;

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

    private static string s_punctuation = ".?!,:;––—´‘/…*& #~\\@^|";

    private static IList<char> s_closingBrackets = new List<char> { ')', '}', ']' };

    /// <summary>
    /// Some characters have to be escaped so that the file path is recognized in a bash skript.
    /// </summary>
    /// <param name="filePath">Path of a file.</param>
    /// <returns>Path of the file where special characters are escaped.</returns>
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

    /// <summary>
    /// In order to find duplicate songs the punctuation has to be removed or normalized as it varies from album to album.
    /// One song is normally released on a lot of different albums or compilations but the song is still the same.
    /// </summary>
    /// <param name="text">Mp3 tag value.</param>
    /// <returns>Mp3 tag value that is normalized with respect to punctuation and the 'and' word.</returns>
    public static string RemovePunctuation(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var result = text.Replace(" and ", "&");
        return new string(result.Where(c => !s_punctuation.Contains(c)).ToArray());
    }

    /// <summary>
    /// In order to find duplicate songs the text in brackets has to be removed as it varies from album to album.
    /// </summary>
    /// <param name="text">Tag value.</param>
    /// <returns>Tag value without the brackets and text between the brackets.</returns>
    public static string RemoveContentInBrackets(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        return MatchingBracketsRegex().Replace(text, "");
    }

    /// <summary>
    /// Spotify mp3 tags do not always match the tags in the local library. 
    /// This function replaces the spotify tag with the version in the local library.
    /// </summary>
    /// <param name="text">Tag value.</param>
    /// <param name="tagMaps">Maps the spotify tag with the local tag.</param>
    /// <returns>A tag that matches with the tag in the local music library.</returns>
    public static string ReplaceSpotifyTagMismatches(this string text, List<MusicBrainzTagMap> tagMaps)
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

    /// <summary>
    /// In order to find duplicate songs text following a certain trigger has to be removed. These appendices vary from album to album.
    /// The song remains the same on each album or compilation.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <returns>Title without the appendix.</returns>
    public static string RemoveTitleAppendix(this string title, NormalizeMode normalizeMode) 
        => RemoveAppendix(title, s_titleAppendixStarters, normalizeMode);

    /// <summary>
    /// In order to find duplicate songs text following a certain trigger has to be removed. These appendices vary from album to album.
    /// The song remains the same on each album or compilation.
    /// </summary>
    /// <param name="artist">Artist.</param>
    /// <returns>Artist without the appendix.</returns>
    public static string RemoveArtistAppendix(this string artist, NormalizeMode normalizeMode) 
        => RemoveAppendix(artist, s_artistAppendixStarters, normalizeMode);

    private static string RemoveAppendix(string text, IList<string> endOfContentIndicators, NormalizeMode normalizeMode)
    {
        var result = text;
        foreach (var endOfTitleIndicator in endOfContentIndicators)
        {
            if (result.Contains(endOfTitleIndicator, StringComparison.OrdinalIgnoreCase))
            {
                result = RemoveAppendix(result, endOfTitleIndicator, normalizeMode);
            }
        }
        return result;
    }

    private static string RemoveAppendix(this string text, string appendixStarter, NormalizeMode normalizeMode)
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
        foreach (var cb in s_closingBrackets)
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

        var minIndexOfNextClosingBracket = s_closingBrackets
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

    /// <summary>
    /// In order to find duplicate songs text following a the trigger word 'feat.' has to be removed. This appendix vary from album to album.
    /// The song remains the same on each album or compilation.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <returns>Title without the featuring appendix.</returns>
    public static string RemoveFeaturingSuffix(this string title)
        => title.Contains("feat.")
            ? title[..title.IndexOf("feat.")].Trim()
            : title;

    [GeneratedRegex(@"\{.*?\}|\[.*?\]|\(.*?\)")]
    private static partial Regex MatchingBracketsRegex();
}
