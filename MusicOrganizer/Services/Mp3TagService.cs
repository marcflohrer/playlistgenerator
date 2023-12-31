// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using MusicOrganizer.Extensions;
using MusicOrganizer.Models;

namespace MusicOrganizer.Services;

public static class Mp3TagService
{
    public record FileTags(string FilePath, Mp3Info Mp3Info);

    public static List<FileTags> GetMp3Info(this List<FileInfo>? files, IList<MusicBrainzTagMap> tagMaps, FileInfo? output)
    {
        if (files == null)
        {
            throw new InvalidOperationException("GetMp3Info: Files must not be null.");
        }
        var fileTags = new List<FileTags>();
        foreach (var f in files)
        {
            var mp3Info = f.GetMp3Info(tagMaps, output);
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

    public static Dictionary<string, SongLocations> NormalizeFileTags(IList<FileTags> mainDirFileTags, IList<MusicBrainzTagMap> tagMaps)
    {
        var mp3Files = new Dictionary<string, SongLocations>();
        foreach (var (mp3Info, normalizedSongKey) in from f in mainDirFileTags
                                                     let mp3Info = f.Mp3Info
                                                     let normalizedTitle = NormalizeSongTag($"{mp3Info.Title}", tagMaps, NormalizeMode.Loose)
                                                     let normalizedInterpret = NormalizeSongTag($"{string.Join(',', mp3Info.Interpret)}", tagMaps, NormalizeMode.Loose)
                                                     let normalizedSongKey = new StringBuilder(normalizedTitle).Append(" + ").Append(normalizedInterpret).ToString()
                                                     select (mp3Info, normalizedSongKey))
        {
            if (!mp3Files.TryGetValue(normalizedSongKey, out var songInfo))
            {
                songInfo = new SongLocations(normalizedSongKey, [mp3Info]);
            }
            else
            {
                songInfo.Mp3Infos.Add(mp3Info);
                songInfo = new SongLocations(normalizedSongKey, songInfo.Mp3Infos);
                mp3Files.Remove(normalizedSongKey);
            }

            mp3Files.Add(normalizedSongKey, songInfo);
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

    public record ScanResult(bool FoundDuplicates, SongsOfArtists SongsOfArtists);

    public static void CreateM3uPlaylists(this ScanResult mainDirScanResult, IList<MusicBrainzTagMap> tagMaps, AppOptions appOptions)
    {
        foreach (var csvFile in appOptions.CsvPlaylistFiles)
        {
            Logger.WriteLine(appOptions.LogFile, $"--------------{Environment.NewLine}PlayList {csvFile.Name}{Environment.NewLine}--------------");
            var spotifyPlaylist = CsvToMp3InfoParser.ToMp3InfoList(csvFile, tagMaps);
            spotifyPlaylist.ToM3uPlaylist(
                tagMaps,
                mainDirScanResult,
                appOptions.ToPlaylistFile(csvFile),
                appOptions.MusicDirectory,
                appOptions.LogFile);
        }
    }

    public static ScanResult CreateDeletionScriptForDuplicates(
        this List<FileTags> fileTags,
        IList<MusicBrainzTagMap> tagMaps,
        FileInfo? deletionScript,
        FileInfo? logFile)
    {
        var mp3Files = new Dictionary<string, SongLocations>();
        foreach (var f in fileTags)
        {
            var mp3Info = f.Mp3Info;
            var normalizedTitle = NormalizeSongTag($"{mp3Info.Title}", tagMaps, NormalizeMode.Loose);
            var normalizedInterpret = NormalizeSongTag($"{string.Join(',', mp3Info.Interpret)}", tagMaps, NormalizeMode.Loose);
            var normalizedSongKey = new StringBuilder(normalizedTitle).Append(" + ").Append(normalizedInterpret).ToString();
            var file = new FileInfo(f.FilePath);
            var addSongInfo = false;
            if (!mp3Files.TryGetValue(normalizedSongKey, out var songInfo))
            {
                songInfo = new SongLocations(normalizedSongKey, [mp3Info]);
                addSongInfo = true;
            }
            else
            {
                var filePathCandidateToAdd = mp3Info.FilePath;
                if (!songInfo.Mp3Infos.Any(mp3 => mp3.FilePath.Equals(filePathCandidateToAdd, StringComparison.InvariantCultureIgnoreCase)))
                {
                    songInfo.Mp3Infos.Add(mp3Info);
                    songInfo = new SongLocations(normalizedSongKey, songInfo.Mp3Infos);
                    mp3Files.Remove(normalizedSongKey);
                    addSongInfo = true;
                }
            }
            if (addSongInfo)
            {
                mp3Files.Add(normalizedSongKey, songInfo);
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
        if (duplicateMp3Files != null && duplicateMp3Files.Count != 0)
        {
            duplicateMp3Files?.CreateDeletionScript(logFile, deletionScript);
            Logger.WriteLine(logFile, $"{DateTime.Now} Warning: There are duplicate mp3 songs.");
            return new ScanResult(true, fileTags.ToSongsOfArtist(tagMaps, logFile));
        }
        return new ScanResult(false, fileTags.ToSongsOfArtist(tagMaps, logFile));
    }

    public static string NormalizeSongTag(
        this string? title,
        IList<MusicBrainzTagMap> musicBrainsTagMap,
        NormalizeMode normalizeMode)
    {
        if (string.IsNullOrEmpty(title))
        {
            return string.Empty;
        }
        var normalizedTag = title
            .ReplaceSpotifyTagErrors(musicBrainsTagMap)
            .ToLowerInvariant()
            .RemoveContentAfterDash(normalizeMode);

        if (normalizeMode == NormalizeMode.StrictInterpret)
        {
            normalizedTag = normalizedTag.RemoveContentAfterAmpersand(normalizeMode);
            normalizedTag = normalizedTag.RemoveContentAfterVersus(normalizeMode);
            normalizedTag = normalizedTag.RemoveContentAfterVersusDot(normalizeMode);
        }
        normalizedTag = normalizedTag.RemoveContentInBrackets()
                                    .RemoveFeaturingSuffix()
                                    .RemovePunctuation()
                                    .ToM3uCompliantPath()
                                    .Text;
        return normalizedTag;
    }
}
