﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MusicOrganizer.Extensions;
using MusicOrganizer.Models;

namespace MusicOrganizer.Services;

public static class Mp3TagService
{
    public record FileTags(string FilePath, Mp3Info Mp3Info);
    public static List<FileTags> GetMp3Info(this List<FileInfo> files, FileInfo? output)
    {
        var fileTags = new List<FileTags>();
        foreach (var f in files)
        {
            var mp3Info = f.GetMp3Info(output);
            if (mp3Info != null)
            {
                fileTags.Add(new(f.FullName, mp3Info));
            }
            else
            {
                Logger.WriteLine(output, $"No tag found: {f.FullName}");
            }
        }
        return fileTags;
    }

    public static List<SongLocation> RemoveDuplicates(this Dictionary<string, SongLocations> songLocations)
    {
        return songLocations.Select(sl => new SongLocation(sl.Key, sl.Value.Mp3Infos.FirstOrDefault()!))
                                                             .ToList();
    }

    public static Dictionary<string, SongLocations> NormalizeFileTags(IList<FileTags> mainDirFileTags)
    {
        var mp3Files = new Dictionary<string, SongLocations>();
        foreach (var f in mainDirFileTags)
        {
            var mp3Info = f.Mp3Info;
            var normalizedTitle = NormalizeSongTag($"{mp3Info.Title} + {string.Join(',', mp3Info.Interpret)}", NormalizeMode.Loose);
            if (!mp3Files.TryGetValue(normalizedTitle, out var songInfo))
            {
                songInfo = new SongLocations(normalizedTitle, [mp3Info]);
            }
            else
            {
                songInfo.Mp3Infos.Add(mp3Info);
                songInfo = new SongLocations(normalizedTitle, songInfo.Mp3Infos);
                mp3Files.Remove(normalizedTitle);
            }
            mp3Files.Add(normalizedTitle, songInfo);
        }

        return mp3Files;
    }

    public static void LogDuplicates(this Dictionary<string, SongLocations> songLocations, FileInfo? logFile)
    {
        var duplicateCount = songLocations?.ToList().Where(ds => ds.Value.Mp3Infos.Count > 1).ToList().Count;
        if (duplicateCount > 0)
        {
            var fileSingularPlural = duplicateCount == 1 ? "file" : "files";
            Logger.WriteLine(logFile, $"{DateTime.Now} Found {duplicateCount} songs with multiple {fileSingularPlural}.");
        }
    }

    public record ScanResult(bool FoundDuplicates, IList<FileTags> FileTags);

    public static void CreateM3uPlaylists(this ScanResult mainDirScanResult, AppOptions appOptions)
    {
        foreach (var csvFile in appOptions.CsvPlaylistFiles)
        {
            Logger.WriteLine(appOptions.LogFile, $"--------------{Environment.NewLine}PlayList {csvFile.Name}{Environment.NewLine}--------------");
            var spotifyPlaylist = CsvToMp3InfoParser.ToMp3InfoList(csvFile);
            spotifyPlaylist.ToM3uPlaylist(
                mainDirScanResult,
                appOptions.ToPlaylistFile(csvFile),
                appOptions.MusicDirectory,
                appOptions.LogFile);
        }
    }

    public static ScanResult CreateDeletionScriptForDuplicates(
        this List<FileTags> fileTags,
        FileInfo? deletionScript,
        FileInfo? logFile)
    {
        var mp3Files = new Dictionary<string, SongLocations>();
        foreach (var f in fileTags)
        {
            var mp3Info = f.Mp3Info;
            var normalizedTitle = NormalizeSongTag($"{mp3Info.Title} + {string.Join(',', mp3Info.Interpret)}", NormalizeMode.Loose);
            var file = new FileInfo(f.FilePath);
            var addSongInfo = false;
            if (!mp3Files.TryGetValue(normalizedTitle, out var songInfo))
            {
                songInfo = new SongLocations(normalizedTitle, [mp3Info]);
                addSongInfo = true;
            }
            else
            {
                var filePathCandidateToAdd = mp3Info.FilePath;
                if (!songInfo.Mp3Infos.Any(mp3 => mp3.FilePath.Equals(filePathCandidateToAdd, StringComparison.InvariantCultureIgnoreCase)))
                {
                    songInfo.Mp3Infos.Add(mp3Info);
                    songInfo = new SongLocations(normalizedTitle, songInfo.Mp3Infos);
                    mp3Files.Remove(normalizedTitle);
                    addSongInfo = true;
                }
            }
            if (addSongInfo)
            {
                mp3Files.Add(normalizedTitle, songInfo);
            }
        }

        var list = mp3Files.ToList().Where(mp3 => mp3.Value.Mp3Infos.Count > 1);
        var duplicateMp3Files = new Dictionary<string, SongLocations>();
        foreach (var kvp in list)
        {
            kvp.Value.Mp3Infos.ForEach(m => m.PrintMp3Info(logFile, "<not decided>"));
            duplicateMp3Files.Add(kvp.Key, kvp.Value);
        }
        duplicateMp3Files?.LogDuplicates(logFile);
        if (duplicateMp3Files != null && duplicateMp3Files.Any())
        {
            duplicateMp3Files?.CreateDeletionScript(logFile, deletionScript);
            Logger.WriteLine(logFile, $"{DateTime.Now} Aborting as there are duplicate mp3 songs.");
            return new ScanResult(true, fileTags);
        }
        return new ScanResult(false, fileTags);
    }

    public static string NormalizeSongTag(this string title, NormalizeMode normalizeMode)
    {
        return title.ToLowerInvariant()
            .RemoveContentAfterDash(normalizeMode)
            .RemoveFeaturingSuffux()
            .RemovePunctuation()
            .RemoveContentInBrackets()
            .ToM3uCompliantPath().Text;
    }
}
