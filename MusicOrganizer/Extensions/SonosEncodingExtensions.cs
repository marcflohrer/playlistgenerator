using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MusicOrganizer.Services;
using TagLib;

namespace MusicOrganizer.Extensions;

public static class SonosEncodingExtensions
{
    public static FileInfo RenameToAsciiOnly(this FileInfo mp3File, IFileService fileService, FileInfo? logFile)
    {
        var cleaned = mp3File.FullName.ToM3uCompliantPath();
        var cleanedFullName = cleaned.Text;
        var changed = cleaned.Changed;

        var cleanedFile = new FileInfo(cleanedFullName);
        var broken = false;
        if (changed)
        {
            var maxCount = 100;
            var index = 0;
            while (fileService.FileExists(cleanedFile))
            {
                cleanedFullName = IncrementDuplicateCounter(cleanedFullName);
                cleanedFile = new FileInfo(cleanedFullName);
                if (index++ > maxCount)
                {
                    broken = true;
                    break;
                }
            }
            if (!broken)
            {
                fileService.MoveFile(mp3File, cleanedFullName, logFile);
            }
        }
        return new FileInfo(cleanedFullName);
    }

    public static string IncrementDuplicateCounter(string cleanedFullName)
    {
        var pattern = @"(.*)(\([0-9]+\))(\.mp3)";
        Regex mp3FileRegexPattern = new Regex(pattern, RegexOptions.Compiled);
        var text = cleanedFullName;
        var match = mp3FileRegexPattern.Match(cleanedFullName);
        if (match.Groups.Count >= 3)
        {
            var countString = match.Groups[2].Value.
                Replace("(", string.Empty).Replace(")", string.Empty);
            if (int.TryParse(countString, out var index))
            {
                cleanedFullName = mp3FileRegexPattern.Replace(text, $"$1({++index})$3");
            }
            else
            {
                cleanedFullName = StartDuplicateCounting(cleanedFullName);
            }
        }
        else
        {
            cleanedFullName = StartDuplicateCounting(cleanedFullName);
        }

        return cleanedFullName;
    }

    private static string StartDuplicateCounting(string cleanedFullName)
        => cleanedFullName.Replace(".mp3", "(1).mp3");

    public static void CleanTags(this TagLib.Tag tag, TagLib.File file, FileInfo? logFile)
    {
        bool changed = false;
        changed = tag.AssignCleanedTag("Album", changed, false, logFile);
        changed = tag.AssignCleanedTag("AlbumArtists", changed, true, logFile);
        changed = tag.AssignCleanedTag("Artists", changed, true, logFile);
        changed = tag.AssignCleanedTag("Title", changed, false, logFile);
        changed = tag.AssignCleanedTag("Performers", changed, true, logFile);
    }

    private static bool AssignCleanedTag(this Tag tag, string tagName,
        bool changed, bool isArray, FileInfo? logger)
    {
        var tagValue = (string)tag.GetPropValue(tagName);
        var cleanedTagValue = tagValue.ToM3uCompliantPath();
        if (cleanedTagValue.Changed)
        {
            Logger.WriteLine(logger, $"{tagName} cleaned: {tagValue} -> {cleanedTagValue.Text}");
            if (isArray)
            {
                tag.SetPropValue(tagName, new[] { cleanedTagValue.Text });
            }
            else
            {
                tag.SetPropValue(tagName, cleanedTagValue.Text);
            }

            changed = true;
        }

        return changed;
    }

    public static object GetPropValue(this object src, string propName)
        => GetPropertyInfo(src, propName)?.GetValue(src, null) ?? throw new InvalidProgramException("Property not found.");

    private static PropertyInfo? GetPropertyInfo(object src, string propName)
        => src.GetType().GetProperty(propName);

    public static void SetPropValue(this object src, string propName, object propValue)
        => GetPropertyInfo(src, propName)?.SetValue(src, propValue, null);
}

