﻿using System;
using System.Text.RegularExpressions;
using AnyAscii;

namespace MusicOrganizer.Extensions;

public static class AsciiExtensions
{
    private static Dictionary<string, string> UnicodeToMultiCharAsciiMap { get => unicodeToMultiCharAsciiMap; set => unicodeToMultiCharAsciiMap = value; }
    private static Dictionary<string, string> unicodeToMultiCharAsciiMap = new Dictionary<string, string>
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
    /*private static readonly Dictionary<string, char> UnicodeToAsciiMap = new Dictionary<string, char>
    {
        {"\u00A1",  'i' },
        {"\u00A2",  'c' },
        {"\u00A3",  'L' },
        {"\u00A4",  'o' },
        {"\u00A5",  'Y' },
        {"\u00A6",  '¦' },
        {"\u00A7",  '§' },
        {"\u00A8",  ' ' },
        {"\u00AA",  'a' },
        {"\u00AC",  '¬' },
        {"\u00AD",  '-' },
        {"\u00AF",  '-' },
        {"\u00B0",  'o' },
        {"\u00B1",  '+' },
        {"\u00B2",  '3' },
        {"\u00B3",  '3' },
        {"\u00B4",  '\'' },
        {"\u00C0",  'A' },
        {"\u00C1",  'A' },
        {"\u00C2",  'A' },
        {"\u00C3",  'A' },
        {"\u00C4",  'A' },
        {"\u00C5",  'A' },
        {"\u00C7",  'C' },
        {"\u00C8",  'E' },
        {"\u00C9",  'E' },
        {"\u00CA",  'E' },
        {"\u00CB",  'E' },
        {"\u00CC",  'I' },
        {"\u00CD",  'I' },
        {"\u00CE",  'I' },
        {"\u00CF",  'I' },
        {"\u00D0",  'D' },
        {"\u00D1",  'N' },
        {"\u00D2",  'O' },
        {"\u00D3",  'O' },
        {"\u00D4",  'O' },
        {"\u00D5",  'O' },
        {"\u00D6",  'O' },
        {"\u00D7",  '×' },
        {"\u00D8",  'O' },
        {"\u00D9",  'U' },
        {"\u00DA",  'U' },
        {"\u00DB",  'U' },
        {"\u00DC",  'U' },
        {"\u00DD",  'Y' },
        {"\u00DE",  'p' },
        {"\u00E0",  'a' },
        {"\u00E1",  'a' },
        {"\u00E2",  'a' },
        {"\u00E3",  'a' },
        {"\u00E5",  'a' },
        {"\u00E7",  'c' },
        {"\u00E8",  'e' },
        {"\u00E9",  'e' },
        {"\u00EA",  'e' },
        {"\u00EB",  'e' },
        {"\u00EC",  'i' },
        {"\u00ED",  'i' },
        {"\u00EE",  'i' },
        {"\u00EF",  'i' },
        {"\u00F0",  'd' },
        {"\u00F1",  'n' },
        {"\u00F2",  'o' },
        {"\u00F3",  'o' },
        {"\u00F4",  'o' },
        {"\u00F5",  'o' },
        {"\u00F7",  '/' },
        {"\u00F8",  'o' },
        {"\u00F9",  'u' },
        {"\u00FA",  'u' },
        {"\u00FB",  'u' },
        {"\u00FC",  'u' },
        {"\u00FD",  'y' },
        {"\u00FE",  'p' },
        {"\u00FF",  'y' },
        {"\u1806",  '-' },
        {"\u1400",  '-' },
        {"\u058A",  '-' },
        {"\u2010",  '-' },
        {"\u2011",  '-' },
        {"\u2012",  '-' },
        {"\u2013",  '-' },
        {"\u2014",  '-' },
        {"\u2015",  '-' },
        {"\u2016",  '|' },
        {"\u2017",  '=' },
        {"\u2018",  '\'' },
        {"\u2019",  '\'' },
        {"\u201A",  ',' },
        {"\u201B",  '\'' },
        {"\u2020",  '+' },
        {"\u2021",  '+' },
        {"\u2022",  '.' },
        {"\u2023",  '>' },
        {"\u2024",  '.' },
        {"\u2032",  '\'' },
        {"\u2033",  '\"' },
        {"\u2034",  '\"' },
        {"\u2035",  '\'' },
        {"\u2036",  '\"' },
        {"\u2037",  '\"' },
        {"\u2039",  '<' },
        {"\u203A",  '>' },
        {"\u203C",  '!' },
        {"\u203E",  '-' },
        {"\u2043",  '-' },
        {"\u2047",  ';' },
        {"\u205D",  ':' },
        {"\u0100",  'A' },
        {"\u0101",  'a' },
        {"\u0102",  'A' },
        {"\u0103",  'a' },
        {"\u0104",  'A' },
        {"\u0105",  'a' },
        {"\u0106",  'C' },
        {"\u0107",  'c' },
        {"\u0108",  'C' },
        {"\u0109",  'c' },
        {"\u010A",  'C' },
        {"\u010B",  'c' },
        {"\u010C",  'C' },
        {"\u010D",  'c' },
        {"\u010E",  'D' },
        {"\u010F",  'd' },
        {"\u0110",  'D' },
        {"\u0111",  'd' },
        {"\u0112",  'E' },
        {"\u0113",  'e' },
        {"\u0114",  'E' },
        {"\u0115",  'E' },
        {"\u0116",  'e' },
        {"\u0117",  'E' },
        {"\u0118",  'E' },
        {"\u0119",  'e' },
        {"\u011A",  'E' },
        {"\u011B",  'e' },
        {"\u011C",  'G' },
        {"\u011D",  'g' },
        {"\u011E",  'G' },
        {"\u011F",  'g' },
        {"\u0120",  'G' },
        {"\u0121",  'g' },
        {"\u0122",  'G' },
        {"\u0123",  'g' },
        {"\u0124",  'H' },
        {"\u0125",  'h' },
        {"\u0126",  'H' },
        {"\u0127",  'h' },
        {"\u0128",  'I' },
        {"\u0129",  'i' },
        {"\u012A",  'I' },
        {"\u012B",  'i' },
        {"\u012C",  'I' },
        {"\u012D",  'i' },
        {"\u012E",  'I' },
        {"\u012F",  'i' },
        {"\u0130",  'I' },
        {"\u0131",  'i' },

        {"\u0134",  'J' },
        {"\u0135",  'j' },
        {"\u0136",  'K' },
        {"\u0137",  'k' },
        {"\u0138",  'k' },
        {"\u0139",  'L' },
        {"\u013A",  'l' },
        {"\u013B",  'L' },
        {"\u013C",  'l' },
        {"\u013D",  'L' },
        {"\u013E",  'l' },
        {"\u013F",  'L' },
        {"\u0140",  'l' },
        {"\u0141",  'L' },
        {"\u0142",  'l' },

        {"\u0143",  'N' },
        {"\u0144",  'n' },
        {"\u0145",  'N' },
        {"\u0146",  'n' },
        {"\u0147",  'N' },
        {"\u0148",  'n' },
        {"\u0149",  'n' },
        {"\u014A",  'N' },
        {"\u014B",  'n' },

        {"\u014C",  'O' },
        {"\u014D",  'o' },
        {"\u014E",  'O' },
        {"\u014F",  'o' },
        {"\u0150",  'O' },
        {"\u0151",  'o' },

        {"\u0154",  'R' },
        {"\u0155",  'r' },
        {"\u0156",  'R' },
        {"\u0157",  'r' },
        {"\u0158",  'R' },
        {"\u0159",  'r' },

        {"\u015A",  'S' },
        {"\u015B",  's' },
        {"\u015C",  'S' },
        {"\u015D",  's' },
        {"\u015E",  'S' },
        {"\u015F",  's' },
        {"\u0160",  'S' },
        {"\u0161",  's' },

        {"\u0162",  'T' },
        {"\u0163",  't' },
        {"\u0164",  'T' },
        {"\u0165",  't' },
        {"\u0166",  'T' },
        {"\u0167",  't' },

        {"\u016A",  'U' },
        {"\u016B",  'u' },
        {"\u016C",  'U' },
        {"\u016D",  'u' },
        {"\u016E",  'U' },
        {"\u016F",  'u' },

        {"\u0172",  'U' },
        {"\u0173",  'u' },

        {"\u0174",  'W' },
        {"\u0175",  'w' },

        {"\u0176",  'Y' },
        {"\u0177",  'y' },
        {"\u0178",  'Y' },

        {"\u0179",  'Z' },
        {"\u017A",  'z' },
        {"\u017B",  'Z' },
        {"\u017C",  'z' },
        {"\u017D",  'Z' },
        {"\u017E",  'z' },

        {"\u017F",  'S' },

        {"\u039E",  'E' },
    };*/

    public record CleanedText(string Text, bool Changed);

    public static CleanedText ToM3uCompliantPath(this string text)
    {
        var pattern = "[^ -~]";
        string result = Regex.Replace(text, pattern, s =>
        {
            var c = (char)s.Value[0];
            var result = UnicodeToMultiCharAsciiMap.TryGetValue(c.ToString(), out var v1)
            ? v1
            : $"{c}";
            return result;
        });
        result = result.Transliterate();
        return new CleanedText(result, text != result);
    }
}

