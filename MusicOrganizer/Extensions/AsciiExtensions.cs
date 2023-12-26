using System.Text.RegularExpressions;
using AnyAscii;

namespace MusicOrganizer.Extensions;

public static class AsciiExtensions
{
    private static Dictionary<string, string> UnicodeToMultiCharAsciiMap { get => s_unicodeToMultiCharAsciiMap; set => s_unicodeToMultiCharAsciiMap = value; }
    private static Dictionary<string, string> s_unicodeToMultiCharAsciiMap = new()
    {
        {"\u00C4",  "Ae" },
        {"\u00D6",  "Oe" },
        {"\u00DC",  "Ue" },
        {"\u00DF",  "ss" },
        {"\u00E4",  "ae" },
        {"\u00F6",  "oe" },
        {"\u00FC",  "ue" },
        {"\u00C6",  "AE" },
        {"\u00E6",  "ae" },
        {"\u00AE",  "(R)" },
        {"\u00A9",  "(c)" },
        {"\u00AB",  "<<" },
        {"\u00BB",  ">>" },
        {"\u042F",  "R" },
        {"\u044F",  "r" },
        {"\u0132",  "ij" },
        {"\u0133",  "ij" },
        {"\u0152",  "OE" },
        {"\u0153",  "oe" },
        {"\u0170",  "Ue" },
        {"\u0171",  "ue" },
        {"\u00BC",  "0,25" },
        {"\u00BD",  "0,5" },
        {"\u00BE",  "0,75" },
        {"\u2010",  "-" },
        {"\u2011",  "-" },
        {"\u2012",  "-" },
        {"\u2013",  "-" },
        {"\u2014",  "-" },
        {"\u2015",  "-" },
        {"\u0016",  "'" },
        {"\u0060",  "'" },
        {"\u00B4",  "'" },
        {"\u2018",  "\'" },
        {"\u2019",  "\'" },
        {"\u201A",  "," },
        {"\u201B",  "\'" },
        {"\u2020",  "+" },
        {"\u2021",  "+" },
        {"\u2022",  "." },
        {"\u2023",  ">" },
        {"\u2024",  "." },
        {"\u2026",  "" },
        {"\u2032",  "\'" },
        {"\u2033",  "\"" },
        {"\u2034",  "\"" },
        {"\u2035",  "\'" },
        {"[",  "(" },
        {"]",  ")'" },
        {".",  "" },
    };

    public record CleanedText(string Text, bool Changed);

    public static CleanedText ToM3uCompliantPath(this string text)
    {
        var pattern = "[^ -~]";
        string result = Regex.Replace(text, pattern, s =>
        {
            var c = s.Value[0];
            var result = UnicodeToMultiCharAsciiMap.TryGetValue(c.ToString(), out var v1)
            ? v1
            : $"{c}";
            return result;
        });
        result = result.Transliterate();
        return new CleanedText(result, text != result);
    }
}

